import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { Box, Button, Set, useModalState, FieldStack, Label } from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
    AddDisclosure,
    DeleteDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import { FieldValues } from "../../layout/Grid"
import { InputField } from "../../input/Input"
import MonacoEditor from "@monaco-editor/react"
import { useFormContext } from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { useUpdateVipLevel } from "../../vip-levels/client"
import { useTrans } from "../../i18n"
import { useDeleteSystemSetting } from "./client"

export const AddButton = ({ onSubmitted }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog dialog={dialog} onSubmitted={onSubmitted} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog }) => {
    return (
        <FormDialog
            defaultValues={{}}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="100vw"
            minWidth="70vw"
            placement="top"
        >
            <EditBody />
        </FormDialog>
    )
}

export const EditButton = ({ record, onSubmitted }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute()
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

const EditDialog = ({ record = {}, onSubmitted, dialog }) => {
    let { id } = record
    let [formData, setSubmit] = useState()
    let { result, error, isLoading, deleteCache, mutate } = useUpdateVipLevel(
        id,
        formData
    )

    let title = useTrans({
        zh: "设定",
        en: "Setting"
    })

    const onSubmit = (data) => setSubmit(data)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{ ...record }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="100vw"
            minWidth="70vw"
            placement="top"
        >
            <EditBody />
        </FormDialog>
    )
}

const EditBody = ({ isAdd = false }) => {
    let { getValues, setValue } = useFormContext()
    let ValueField = "value"
    let value = getValues(ValueField)
    return (
        <FieldStack orientation="vertical">
            {isAdd && <InputField name="key" label="Key" />}
            {!isAdd && <InputField value={record?.key} disabled label="Key" />}
            <MonacoEditor
                defaultLanguage="JSON"
                height="50vh"
                defaultValue={value}
                onChange={(value) => {
                    setValue(value)
                }}
                options={{
                    contextmenu: false
                }}
            />
        </FieldStack>
    )
}

export const DeleteButton = ({ record }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <DeleteDisclosure {...dialog} />
            {dialog.visible && <DeleteDialog record={record} dialog={dialog} />}
        </>
    )
}

const DeleteDialog = ({ record, dialog }) => {
    let { id } = record
    let { deleteId, setDeleteId } = useState()
    let {
        result,
        isLoading,
        error,
        mutate,
        deleteCache
    } = useDeleteSystemSetting(deleteId)

    return <FormDialog dialog={dialog}>TODO</FormDialog>
}
