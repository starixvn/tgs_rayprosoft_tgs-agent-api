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
import { useForm, useFormContext } from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddBetLimit, useUpdateBetLimit } from "../../betlimit/client"
import { CurrencySelectField } from "../../currency/CurrencySelect"
import { NumberInputField } from "../../input/NumberInput"

export const EditBetLimitDisclosure = ({ record, onSubmitted }) => {
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

    let { result, isLoading, error, mutate, deleteCache } = useUpdateBetLimit(
        record?.id,
        formData
    )

    let title = useTrans({
        zh: "编辑限红",
        en: "Edit Bet Limit"
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

export const AddBetLimitDisclosure = ({ onSubmitted }) => {
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

    let { result, isLoading, error, mutate, deleteCache } = useAddBetLimit(
        formData
    )

    let title = useTrans({
        zh: "新增限红",
        en: "Create Bet Limit"
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
    let { watch } = useFormContext()
    let minBet = watch("minBet") ?? 0

    let minBetLabel = useTrans({
        zh: "最低投注",
        en: "Min Bet"
    })

    let maxBetLabel = useTrans({
        zh: "最高投注",
        en: "Max Bet"
    })

    let maxBetPerMatchLabel = useTrans({
        zh: "单场最高投注",
        en: "Max Bet Per Match"
    })

    let isEdit = !isAdd
    return (
        <Box>
            {isEdit && <Input type="hidden" name="id" />}
            <FieldStack>
                <CurrencySelectField name="currencyCode" required />
                <NumberInputField
                    name="minBet"
                    min="0"
                    label={minBetLabel}
                    required
                />
                <NumberInputField
                    name="maxBet"
                    min={minBet}
                    label={maxBetLabel}
                    required
                />
                <NumberInputField
                    name="maxBetPerMatch"
                    min="0"
                    label={maxBetPerMatchLabel}
                />
            </FieldStack>
        </Box>
    )
}
