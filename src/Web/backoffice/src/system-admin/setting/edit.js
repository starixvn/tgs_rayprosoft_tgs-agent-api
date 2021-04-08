import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Button,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import { Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { EditDisclosure, ViewDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import MonacoEditor, { DiffEditor } from "@monaco-editor/react"
import Input, { InputField } from "../../input/Input"
import { useUpdateSystemSetting } from "./client"
import { useForm, Controller } from "react-hook-form"
import { TextareaField } from "../../textarea"

export const EditSettingDisclosure = ({ record, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({ record, onSubmitted, dialog, ...props }) => {
    let { key, value, description } = record

    let title = useTrans({
        zh: "编辑设定",
        en: "Edit Setting"
    })

    let [formData, setSubmit] = useState()
    let {
        result,
        isLoading,
        error,
        mutate,
        deleteCache
    } = useUpdateSystemSetting(key, formData)

    let formMethods = useForm({
        defaultValues: {
            key,
            value: stringifyJSON(value),
            description
        }
    })
    let { control, setValue } = formMethods

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            formMethods={formMethods}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            showActionButtons
            maxWidth="90vw"
            width="90vw"
            height="90vh"
            maxHeight="90vh"
            placement="top"
            css={css`
                & .bb-DialogContent {
                    flex: 1 100%;
                    max-height: unset;

                    > .bb-Box {
                        display: flex;
                        flex-direction: column;
                        justify-content: space-between;
                    }
                }
            `}
        >
            <FieldStack orientation="vertical" flex="1 100%">
                <FieldStack orientation="horizontal">
                    <InputField name="key" label="Key" disabled />
                    <InputField name="description" label="Description" />
                </FieldStack>
                <Controller
                    control={control}
                    name="value"
                    render={({
                        field: { onChange, onBlur, value, name, ref }
                    }) => (
                        <MonacoEditor
                            defaultLanguage="JSON"
                            height="60vh"
                            value={value}
                            options={{
                                readOnly: false,
                                scrollBeyondLastLine: true,
                                minimap: { enabled: false },
                                contextmenu: false,
                                hideCursorInOverviewRuler: true
                            }}
                            onChange={(value, event) => {
                                setValue(name, value)
                            }}
                            css={css`
                                border: 1px solid
                                    var(--bb-palette-white900, #000000);
                                border-radius: 6px;
                            `}
                        />
                    )}
                />
            </FieldStack>
        </FormDialog>
    )
}

export const ViewSettingDisclosure = ({ record }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
        </>
    )
}

const ViewDialog = ({ record, dialog }) => {
    let { key, value, description } = record
    let title = useTrans({
        zh: "查看设定",
        en: "View Setting"
    })

    let formattedValue = stringifyJSON(value)

    return (
        <Dialog.Modal
            title={title}
            hideOnClickOutside={false}
            showCloseButton
            showActionButtons={false}
            {...dialog}
            maxWidth="90vw"
            width="90vw"
            height="90vh"
            maxHeight="90vh"
            placement="top"
            css={css`
                & .bb-DialogContent {
                    flex: 1 100%;
                    max-height: unset;

                    > .bb-Box {
                        display: flex;
                        flex-direction: column;
                        justify-content: space-between;
                    }
                }
            `}
        >
            <FieldStack orientation="vertical" flex="1 100%">
                <FieldStack orientation="horizontal">
                    <InputField label="Key" value={key} disabled />
                    <InputField
                        label="Description"
                        value={description}
                        disabled
                    />
                </FieldStack>
                <MonacoEditor
                    defaultLanguage="JSON"
                    height="60vh"
                    value={formattedValue}
                    options={{
                        readOnly: true,
                        scrollBeyondLastLine: true,
                        minimap: { enabled: false },
                        contextmenu: false,
                        hideCursorInOverviewRuler: true
                    }}
                    css={css`
                        border: 1px solid var(--bb-palette-white900, #000000);
                        border-radius: 6px;
                    `}
                />
            </FieldStack>
        </Dialog.Modal>
    )
}

const stringifyJSON = (value) => {
    try {
        if (typeof value === "string") {
            let parsed = JSON.parse(value)
            return JSON.stringify(parsed, null, 4)
        }
    } catch {
        //ignore
    }

    return value
}
