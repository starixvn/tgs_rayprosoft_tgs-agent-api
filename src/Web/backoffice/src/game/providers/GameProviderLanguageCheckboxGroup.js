import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, useModalState, Icon } from "bumbag"
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
import { ConfirmText } from "../../button/ActionButtons"
import { faLanguage } from "@fortawesome/free-solid-svg-icons"

export const GameProviderLanguageCheckboxGroupTable = ({
    name = "gameProviderLanguages",
    fieldArrayName = "languages",
    width = "100%",
    disabled = false,
    ...props
}) => {
    let { control, setValue, watch, register } = useFormContext()
    let languages = watch(fieldArrayName)
    const { fields } = useFieldArray({
        control,
        name: fieldArrayName,
        keyName: "_id"
    })

    let selection = watch(name) ?? {}
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
                                <Table.HeadCell textAlign="center">
                                    <Trans zh="设定" en="Setting" />
                                </Table.HeadCell>
                            }
                        />
                    </Table.Head>
                    <Table.Body>
                        {fields.map((field) => (
                            <LanguageCheckboxGroupTableRow
                                key={field._id}
                                name={name}
                                disabled={disabled}
                                field={field}
                                checkedFieldName={checkedFieldName}
                                addonAfter={
                                    <Table.Cell textAlign="center">
                                        <Set
                                            orientation="horizontal"
                                            alignX="center"
                                        >
                                            <Controller
                                                control={control}
                                                name={cultureCodeFieldName(
                                                    name,
                                                    field
                                                )}
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
                                                    let dialog = useModalState()
                                                    return (
                                                        <>
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
                                                        </>
                                                    )
                                                }}
                                            />
                                        </Set>
                                    </Table.Cell>
                                }
                            />
                        ))}
                    </Table.Body>
                </Table>
            </Set>
        )
    }, [JSON.stringify(selection), JSON.stringify(fields)])
}

const CodeDialog = ({ value, dialog, onChange }) => {
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
                submitText={<ConfirmText />}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="max-content"
            >
                <Input name="code" defaultValue={value} />
            </FormDialog>
        </Backdrop>
    )
}

const checkedFieldName = (name, checkboxField) =>
    `${name}.${checkboxField.cultureCode}.checked`

const cultureCodeFieldName = (name, checkboxField) => {
    let result = `${name}.${checkboxField.cultureCode}.gameProviderCultureCode`
    return result
}
