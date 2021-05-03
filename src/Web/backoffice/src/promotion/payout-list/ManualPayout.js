import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, useModalState, FieldStack } from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import Disclosure, { ViewDisclosure } from "../../dialog/Disclosure"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import FormDialog from "../../form/FormDialog"
import clone from "clone-deep"
import { FieldValues } from "../../layout/Grid"
import { PromotionTypeLabel, usePromotionGroupLabelOf } from "../PromotionType"
import { useSendPromotionPayout } from "./client"
import {
    CloseButtonFooter,
    ConfirmCancelFooter
} from "../../form/ConfirmButtonFooter"

export const PromotionManualPayoutDisclosure = ({
    brandId,
    record,
    onSubmitted
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="手动派发" en="Manual Payout" />
            </Disclosure>
            {dialog.visible && (
                <ManualPayoutDialog
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const ViewPromotionManualPayoutDisclosure = ({ brandId, record }) => {
    let dialog = useModalState()

    /*
        let isDisplay = useIsDisplayMenuByRoute([View])
        if (!isDisplay) {
            return null
        }
        */

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <ViewPayoutDialog
                    brandId={brandId}
                    record={record}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ManualPayoutDialog = ({ brandId, record, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "手动派发",
        en: "Manual Payout"
    })

    let [promotionId, setPromotionId] = useState()
    let {
        isLoading,
        result,
        error,
        mutate,
        deleteCache
    } = useSendPromotionPayout(promotionId)

    const onSubmit = () => {
        setPromotionId(record?.promotionId)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setPromotionId(undefined)
    }

    return (
        <FormDialog
            defaultValues={clone(record)}
            onSubmit={onSubmit}
            onSubmitted={onSubmitted}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            showActionButtons={false}
            footer={
                <ConfirmCancelFooter
                    confirmText={<Trans zh="手动派发" en="Manual Payout" />}
                    dialog={dialog}
                />
            }
            title={title}
            dialog={dialog}
        >
            <FormBody record={record} />
        </FormDialog>
    )
}

const ViewPayoutDialog = ({ brandId, record, dialog }) => {
    let title = useTrans({
        zh: "查看",
        en: "View"
    })

    return (
        <FormDialog
            defaultValues={clone(record)}
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} />}
            title={title}
            dialog={dialog}
        >
            <FormBody record={record} />
        </FormDialog>
    )
}

const FormBody = ({ record }) => {
    let promotionTypeGroup = usePromotionGroupLabelOf(record?.promotionType)
    return (
        <FieldStack marginLeft="1rem">
            <FieldValues>
                <Trans zh="用户名" en="Username" />
                <Box alignY="center">{record?.username}</Box>

                <Trans zh="活动名称 " en="Promotion Name" />
                <Box alignY="center">{record?.promotionName}</Box>

                <Trans zh="活动类型  " en="Promotion Type Group" />
                <Box alignY="center">{promotionTypeGroup}</Box>

                <Trans zh="活动子类型  " en="Promotion Type" />
                <Box alignY="center">
                    <PromotionTypeLabel value={record?.promotionType} />
                </Box>

                <Trans zh="生成时间" en="Created Time" />
                <Box alignY="center">
                    <FormattedDateTime
                        value={record?.createdOn}
                        placeholder="-"
                    />
                </Box>

                <Trans zh="派发时间" en="Sent On" />
                <Box alignY="center">
                    <FormattedDateTime value={record?.sentOn} placeholder="-" />
                </Box>

                <Trans zh="派发金额" en="Payout Amount" />
                <Box alignY="center">
                    <FormattedNumberDecimal value={record?.bonusAmount} />
                </Box>
            </FieldValues>
        </FieldStack>
    )
}
