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
import { AddDisclosure, EditDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import { useForm } from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddCurrency, useUpdateCurrency } from "../../currency/client"

export const EditCurrencyDisclosure = ({ record, onSubmitted }) => {
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

const EditDialog = ({ record = {}, onSubmitted, dialog, ...props }) => {
    let [formData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useUpdateCurrency(
        record?.code,
        formData
    )

    let title = useTrans({
        zh: "Edit Currency",
        en: "Edit Currency"
    })

    const onSubmit = (data) => {
        let formData = { ...data, code: record?.code }
        setSubmit(formData)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={clone(record)}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
        >
            <FormBody />
        </FormDialog>
    )
}

export const AddCurrencyDisclosure = ({ onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog, ...props }) => {
    let [formData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useAddCurrency(
        formData
    )

    let title = useTrans({
        zh: "Create Currency",
        en: "Create Currency"
    })

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{}}
            onSubmit={setSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
        >
            <FormBody isAdd />
        </FormDialog>
    )
}

const FormBody = ({ isAdd = false }) => {
    let codeLabel = useTrans({
        zh: "代码",
        en: "Code"
    })

    let nameLabel = useTrans({
        zh: "名称",
        en: "Name"
    })

    let nativeNameLabel = useTrans({
        zh: "本地名称",
        en: "Native Name"
    })

    let symbolLabel = useTrans({
        zh: "符号",
        en: "Symbol"
    })

    let isEdit = !isAdd
    return (
        <>
            <FieldStack>
                <InputField name="code" label={codeLabel} disabled={isEdit} />
                <InputField name="name" label={nameLabel} />
                <InputField name="nativeName" label={nativeNameLabel} />
                <InputField name="symbol" label={symbolLabel} />
            </FieldStack>
        </>
    )
}
