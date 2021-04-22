import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Box } from "bumbag"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { Trans } from "../i18n"
import { isAllChecked, isAllPropChecked } from "../checkbox/isAllChecked"
import { Input } from "../input/Input"

export const CurrencyCheckboxGroupTable = ({
    name = "currencies",
    fieldArrayContext,
    width = "100%",
    disabled = false,
    ...props
}) => {
    let { control, setValue, watch } = useFormContext()

    let { fields } = fieldArrayContext

    let selection = watch(name)
    let allChecked = isAllPropChecked(selection)

    return useMemo(() => {
        return (
            <Set orientation="vertical" width={width}>
                <Table width={width}>
                    <Table.Head>
                        <CurrencyCheckboxGroupTableHeadRow
                            name={name}
                            fields={fields}
                            checkedFieldName={toCheckedFieldName}
                            allChecked={allChecked}
                            disabled={disabled}
                        />
                    </Table.Head>
                    <Table.Body>
                        {fields.map((field, index) => (
                            <CurrencyCheckboxGroupTableRow
                                key={field._id}
                                name={name}
                                disabled={disabled}
                                field={field}
                                index={index}
                            />
                        ))}
                    </Table.Body>
                </Table>
            </Set>
        )
    }, [JSON.stringify(selection), JSON.stringify(fields)])
}

export const CurrencyCheckboxGroupTableHeadRow = ({
    name,
    fields,
    checkedFieldName = toCheckedFieldName,
    allChecked,
    disabled,
    addonAfter
}) => {
    let { control, setValue, watch } = useFormContext()

    return useMemo(() => {
        return (
            <Table.Row>
                <Table.HeadCell>
                    <Checkbox
                        checked={allChecked}
                        onChange={(event) => {
                            let checked = event.target.checked
                            fields.forEach((field, index) => {
                                setValue(
                                    checkedFieldName(name, field, index),
                                    checked
                                )
                            })
                        }}
                        disabled={disabled}
                    />
                </Table.HeadCell>
                <Table.HeadCell textAlign="center">
                    <Trans zh="代码" en="Code" />
                </Table.HeadCell>
                <Table.HeadCell textAlign="center">
                    <Trans zh="币种" en="Currency" />
                </Table.HeadCell>
                {addonAfter}
            </Table.Row>
        )
    }, [JSON.stringify(fields), allChecked])
}

export const CurrencyCheckboxGroupTableRow = ({
    name,
    disabled,
    field,
    index,
    checkedFieldName = toCheckedFieldName,
    addonAfter
}) => {
    let formContext = useFormContext()

    return useMemo(() => {
        return (
            <Table.Row>
                <Table.Cell textAlign="center">
                    <Box>
                        <Input
                            type="hidden"
                            name={`${name}.${index}.code`}
                            value={field.code}
                        />
                        <Checkbox
                            key={field._id}
                            name={checkedFieldName(name, field, index)}
                            checkboxRef={formContext?.register?.()}
                            defaultChecked={field.checked}
                            disabled={disabled}
                        />
                    </Box>
                </Table.Cell>
                <Table.Cell textAlign="center">{field.code}</Table.Cell>
                <Table.Cell textAlign="center">{field.name}</Table.Cell>
                {addonAfter}
            </Table.Row>
        )
    }, [field])
}

export const toCheckedFieldName = (name, field, index) =>
    `${name}.${index}.checked`
