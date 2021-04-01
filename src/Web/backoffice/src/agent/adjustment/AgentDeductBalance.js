import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure, { AddDisclosure } from "../../dialog/Disclosure"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import {
    CloseButtonFooter,
    ConfirmButtonFooter,
    ConfirmCancelFooter
} from "../../form/ConfirmButtonFooter"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import { useAgentDeductBalance } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import dateTimeColumn from "../../table/column/dateTime"
import {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import { formatDateTime } from "../../date/format"
import { FieldValues } from "../../layout/Grid"
import { formatNumberDecimal } from "../../number/format"
import Separator from "../../layout/Separator"
import { Update } from "./menu"
import FormDialog from "../../form/FormDialog"
import AddBalanceTypeSelect from "./AgentAddBalanceType"
import { useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import DeductBalanceType from "./AgDeductBalanceType"
import { DeductBalanceTargetSelect } from "./AgentDeductBalanceTarget"

export const AgentDeductBalanceDisclosure = ({
    brandId,
    username,
    onSubmitted,
    ...props
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="扣钱" en="Deduct Balance" />
            </Disclosure>
            {dialog.visible && (
                <DeductDialog
                    brandId={brandId}
                    username={username}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const DeductDialog = ({ brandId, username, onSubmitted, dialog }) => {
    let [formData, setSubmit] = useState()
    let {
        result,
        isLoading,
        error,
        mutate,
        deleteCache
    } = useAgentDeductBalance({ brandId, username }, formData)

    let title = useTrans({
        zh: "余额扣分",
        en: "Deduct Balance"
    })

    const onSubmit = (data) => setSubmit(data)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

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
            showActionButtons={false}
            footer={
                <ConfirmCancelFooter
                    isLoading={isLoading}
                    disabled={isLoading}
                    dialog={dialog}
                />
            }
        >
            <FormBody brandId={brandId} username={username} />
        </FormDialog>
    )
}

const FormBody = ({ brandId, username }) => {
    let { watch } = useFormContext()

    let typeFieldName = "type"
    let type = watch(typeFieldName)

    return (
        <>
            <Input type="hidden" name="brandId" value={brandId} />
            <Input type="hidden" name="username" value={username} />
            <FieldValues margin="1rem">
                <Text.Block alignY="center">
                    <Trans zh="代理用户名" en="Agent Username" />
                </Text.Block>
                <Text.Block alignY="center">{username}</Text.Block>

                <Text.Block alignY="center">
                    <Trans zh="交易类型" en="Transaction Type" />
                </Text.Block>
                <AddBalanceTypeSelect name={typeFieldName} required />

                {type == DeductBalanceType.Transfer && (
                    <>
                        <Text.Block alignY="center">
                            <Trans zh="转账对象" en="Transfer Target" />
                        </Text.Block>
                        <DeductBalanceTargetSelect name="target" required />

                        <Text.Block alignY="center">
                            <Trans zh="转账用户名" en="Transfer Username" />
                        </Text.Block>
                        <Input name="transferUsername" required />
                    </>
                )}

                <Text.Block alignY="center">
                    <Trans zh="交易金额" en="Amount" />
                </Text.Block>

                <Input type="number" name="amount" min="0" required />

                <Text.Block alignY="center">
                    <Trans zh="备注" en="Remark" />
                </Text.Block>

                <Input name="remark" />
            </FieldValues>
        </>
    )
}
