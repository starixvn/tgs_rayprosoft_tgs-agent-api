import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, useModalState, Icon, Box } from "bumbag"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import {
    LanguageCheckboxGroupTableHeadRow,
    LanguageCheckboxGroupTableRow
} from "../../language/LanguageCheckboxGroup"
import { Trans, useTrans } from "../../i18n"
import Input from "../../input/Input"
import Backdrop from "../../dialog/Backdrop"
import FormDialog from "../../form/FormDialog"
import Disclosure from "../../dialog/Disclosure"
import { CloseText, ConfirmText } from "../../button/ActionButtons"
import { faLanguage } from "@fortawesome/free-solid-svg-icons"

export const GameProviderLanguageCheckboxGroupTable = ({
    name = "languages",
    fieldArrayContext,
    width = "100%",
    disabled = false,
    showCultureCode = true,
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
                        <LanguageCheckboxGroupTableHeadRow
                            name={name}
                            fields={fields}
                            checkedFieldName={checkedFieldName}
                            allChecked={allChecked}
                            disabled={disabled}
                            addonAfter={
                                showCultureCode ? (
                                    <Table.HeadCell textAlign="center">
                                        <Trans zh="设定" en="Setting" />
                                    </Table.HeadCell>
                                ) : null
                            }
                        />
                    </Table.Head>
                    <Table.Body>
                        {fields.map((field, index) => {
                            return (
                                <LanguageCheckboxGroupTableRow
                                    key={field._id}
                                    name={name}
                                    disabled={disabled}
                                    field={field}
                                    index={index}
                                    checkedFieldName={checkedFieldName}
                                    addonAfter={
                                        showCultureCode ? (
                                            <Table.Cell textAlign="center">
                                                <Controller
                                                    control={control}
                                                    name={cultureCodeFieldName(
                                                        name,
                                                        field,
                                                        index
                                                    )}
                                                    defaultValue={
                                                        field.brandCultureCode
                                                    }
                                                    shouldUnregister={true}
                                                    render={({
                                                        field: {
                                                            onChange,
                                                            onBlur,
                                                            value,
                                                            name: fieldName,
                                                            ref
                                                        },
                                                        fieldState: {
                                                            invalid,
                                                            isTouched,
                                                            isDirty
                                                        }
                                                    }) => {
                                                        let dialog =
                                                            useModalState()
                                                        return (
                                                            <Box>
                                                                <Input
                                                                    type="hidden"
                                                                    name={`${name}[${index}].cultureCode`}
                                                                    value={
                                                                        field.cultureCode
                                                                    }
                                                                />
                                                                <Disclosure
                                                                    {...dialog}
                                                                >
                                                                    <Icon
                                                                        icon={
                                                                            faLanguage
                                                                        }
                                                                        type="font-awesome"
                                                                    />
                                                                </Disclosure>
                                                                {dialog.visible && (
                                                                    <CodeDialog
                                                                        value={
                                                                            value
                                                                        }
                                                                        disabled={
                                                                            disabled
                                                                        }
                                                                        dialog={
                                                                            dialog
                                                                        }
                                                                        onChange={(
                                                                            code
                                                                        ) => {
                                                                            setValue(
                                                                                fieldName,
                                                                                code
                                                                            )
                                                                        }}
                                                                    />
                                                                )}
                                                            </Box>
                                                        )
                                                    }}
                                                />
                                            </Table.Cell>
                                        ) : null
                                    }
                                />
                            )
                        })}
                    </Table.Body>
                </Table>
            </Set>
        )
    }, [JSON.stringify(selection), JSON.stringify(fields)])
}

const CodeDialog = ({ value, disabled, dialog, onChange }) => {
    let title = useTrans({
        zh: `游戏商语言代码`,
        en: `Game Provider Culture Code`
    })
    return (
        <Backdrop {...dialog}>
            <FormDialog
                stopPropagation={true}
                onSubmit={({ code }) => {
                    onChange(code)
                    dialog.hide()
                }}
                submitText={disabled ? <CloseText /> : <ConfirmText />}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="max-content"
            >
                <Input name="code" defaultValue={value} disabled={disabled} />
            </FormDialog>
        </Backdrop>
    )
}

const checkedFieldName = (name, field, index) => `${name}.${index}.checked`

const cultureCodeFieldName = (name, field, index) => {
    let result = `${name}.${index}.gameProviderCultureCode`
    return result
}
