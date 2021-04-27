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
import { useFormContext } from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { useTrans } from "../../i18n"
import {
    BetLimitMaxBetPerMatchTrans,
    BetLimitMaxBetTrans,
    BetLimitMinBetTrans,
    useBetLimitTrans
} from "../../betlimit/trans"
import { useUpdateBetLimit } from "../../betlimit/client"
import { CurrencySelectField } from "../../currency/CurrencySelect"
import { NumberInputField } from "../../input/NumberInput"

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
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
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
    let [formData, setSubmit] = useState()
    let { result, error, isLoading, deleteCache, mutate } = useUpdateBetLimit(
        record?.id,
        formData
    )

    let title = useTrans({
        zh: "编辑限红",
        en: "Edit Bet Limit"
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

const EditBody = ({}) => {
    return (
        <FieldStack>
            <CurrencySelectField name="currencyCode" required />
            <NumberInputField
                name="minBet"
                min="0"
                required
                label={<BetLimitMinBetTrans />}
            />

            <NumberInputField
                name="maxBet"
                min="0"
                required
                label={<BetLimitMaxBetTrans />}
            />

            <NumberInputField
                name="maxBetPerMatch"
                min="0"
                label={<BetLimitMaxBetPerMatchTrans />}
            />
        </FieldStack>
    )
}
