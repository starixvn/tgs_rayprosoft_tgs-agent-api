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
import { useAddLanguage, useUpdateLanguage } from "../../language/client"

export const EditLangDisclosure = ({ record, onSubmitted }) => {
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

    let { result, isLoading, error, mutate, deleteCache } = useUpdateLanguage(
        record?.cultureCode,
        formData
    )

    let title = useTrans({
        zh: "编辑语系",
        en: "Edit Language"
    })

    const onSubmit = (data) => {
        let formData = { ...data, cultureCode: record?.cultureCode }
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

export const AddLangDisclosure = ({ onSubmitted }) => {
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

    let { result, isLoading, error, mutate, deleteCache } = useAddLanguage(
        formData
    )

    let title = useTrans({
        zh: "新增语系",
        en: "Create Language"
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
    let cultureCodeLabel = useTrans({
        zh: "Culture Code",
        en: "Culture Code"
    })

    let languageCodeLabel = useTrans({
        zh: "Language Code",
        en: "Language Code"
    })

    let nameLabel = useTrans({
        zh: "名称",
        en: "Name"
    })

    let localizedNameLabel = useTrans({
        zh: "本地名称",
        en: "Localized Name"
    })

    let isEdit = !isAdd
    return (
        <>
            <FieldStack>
                <InputField
                    name="cultureCode"
                    label={cultureCodeLabel}
                    disabled={isEdit}
                />
                <InputField name="languageCode" label={languageCodeLabel} />
                <InputField name="name" label={nameLabel} />
                <InputField name="localizedName" label={localizedNameLabel} />
            </FieldStack>
        </>
    )
}
