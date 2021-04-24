import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Text,
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
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import { useForm, useFormContext } from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddPromotion, useUpdatePromotion } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import AlertDialog from "../../dialog/Alert"

export const AddPromotionDisclosure = ({ onSubmitted }) => {
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

    let { result, isLoading, error, mutate, deleteCache } = useAddPromotion(
        formData
    )

    let title = useTrans({
        zh: "新增活动",
        en: "Create Promotion"
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

export const EditPromotionDisclosure = ({
    brandId,
    record,
    onSubmitted,
    ...props
}) => {
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
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({ brandId, record, onSubmitted, dialog, ...props }) => {
    let [formData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useUpdatePromotion(
        record?.id,
        formData
    )

    let title = useTrans({
        zh: "编辑活动",
        en: "Edit Promotion"
    })

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

export const ViewPromotionDisclosure = ({
    brandId,
    record,
    onSubmitted,
    ...props
}) => {
    let dialog = useModalState()

    // let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    // if (!isDisplay) {
    //     return null
    // }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <ViewDialog brandId={brandId} record={record} dialog={dialog} />
            )}
        </>
    )
}

const ViewDialog = ({ brandId, record, dialog, ...props }) => {
    // let { data, isLoading, error, mutate, deleteCache } = usePromotion(
    //     record?.id
    // )

    let title = useTrans({
        zh: "查看活动",
        en: "View Promotion"
    })

    // if (isLoading) {
    //     return <LoadingBackdrop {...dialog} />
    // }

    // if (error) {
    //     return <AlertDialog onClickClose={dialog.hide} />
    // }

    return (
        <FormDialog defaultValues={clone(record)} title={title} dialog={dialog}>
            <FormBody brandId={brandId} record={record} disabled />
        </FormDialog>
    )
}

const FormBody = ({ brandId, record, readonly = false, isAdd = false }) => {
    let disabled = readonly
    let editable = !readonly
    return (
        <FieldStack marginLeft="1rem">
            <FieldValues>
                <NormalLabel>
                    <Trans zh="活动名称" en="Promotion Name" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && <Input name="name" />}
                    {readonly && (
                        <Text.Block alignY="center">{record?.name}</Text.Block>
                    )}
                </Box>

                <NormalLabel>
                    <Trans zh="活动时间" en="Promotion Time" />
                </NormalLabel>
                <Box alignY="center">
                    <DateInputRange
                        startName="from"
                        endName="to"
                        disabled={disabled}
                    />
                </Box>

                <NormalLabel>
                    <Trans zh="活动用户群" en="Promotion VIP Levels" />
                </NormalLabel>
                <Box alignY="center">
                    <VipLevelCheckboxGroup name="vipIds" brandId={brandId} />
                </Box>
            </FieldValues>
        </FieldStack>
    )
}
