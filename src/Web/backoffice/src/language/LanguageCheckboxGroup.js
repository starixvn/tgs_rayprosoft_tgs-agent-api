import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Box } from "bumbag"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllPropChecked, isAllChecked } from "../checkbox/isAllChecked"
import { Trans } from "../i18n"
import Input from "../input/Input"

export const LanguageCheckboxGroupTable = ({
    name = "languages",
    fieldArrayContext,
    width = "100%",
    disabled = false,
    ...props
}) => {
    let { watch } = useFormContext()
    const { fields } = fieldArrayContext

    let checkedFieldNames = fields.map((field, index) =>
        defaultCheckedFieldName(name, field, index)
    )

    let selection = watch(checkedFieldNames)
    let allChecked = isAllChecked(selection)

    return useMemo(() => {
        return (
            <Set orientation="vertical" width={width}>
                <Table width={width}>
                    <Table.Head>
                        <LanguageCheckboxGroupTableHeadRow
                            name={name}
                            fields={fields}
                            checkedFieldName={defaultCheckedFieldName}
                            allChecked={allChecked}
                            disabled={disabled}
                        />
                    </Table.Head>
                    <Table.Body>
                        {fields.map((field, index) => (
                            <LanguageCheckboxGroupTableRow
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

export const LanguageCheckboxGroupTableHeadRow = ({
    name,
    fields,
    checkedFieldName = defaultCheckedFieldName,
    allChecked,
    disabled,
    addonAfter
}) => {
    let { control, setValue, watch } = useFormContext()
    return useMemo(() => {
        return (
            <Table.Row>
                <Table.HeadCell textAlign="center">
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
                    <Trans zh="语言" en="Language" />
                </Table.HeadCell>
                {addonAfter}
            </Table.Row>
        )
    }, [JSON.stringify(fields), allChecked])
}

export const LanguageCheckboxGroupTableRow = ({
    name,
    disabled,
    field,
    index,
    checkedFieldName = defaultCheckedFieldName,
    addonAfter
}) => {
    let { register, setValue } = useFormContext()
    let checkboxFieldName = checkedFieldName(name, field, index)
    let { ref: checkboxRef, ...checkboxFormProps } = register(checkboxFieldName)

    let editable = !disabled
    return useMemo(() => {
        return (
            <Table.Row>
                <Table.Cell textAlign="center">
                    <Box>
                        <Input
                            type="hidden"
                            name={`${name}.${index}.cultureCode`}
                            value={field.cultureCode}
                        />
                        {disabled && (
                            <>
                                <Checkbox
                                    display="none"
                                    checked={field.checked}
                                    disabled
                                    checkboxRef={checkboxRef}
                                    {...checkboxFormProps}
                                />
                                <Checkbox checked={field.checked} disabled />
                            </>
                        )}
                        {editable && (
                            <Checkbox
                                checkboxRef={checkboxRef}
                                defaultChecked={field.checked}
                                {...checkboxFormProps}
                                onChange={(event) => {
                                    let checked = event.target.checked
                                    setValue(checkboxFieldName, checked)
                                }}
                            />
                        )}
                    </Box>
                </Table.Cell>
                <Table.Cell textAlign="center">{field.cultureCode}</Table.Cell>
                <Table.Cell textAlign="center">{field.name}</Table.Cell>
                {addonAfter}
            </Table.Row>
        )
    }, [field])
}

const defaultCheckedFieldName = (name, field, index) =>
    `${name}.${index}.checked`
