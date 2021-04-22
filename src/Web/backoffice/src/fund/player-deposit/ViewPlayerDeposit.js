import React from "react"
import {
    Spinner,
    useModalState,
    FieldStack,
    Box,
    Text,
    Dialog,
    Modal,
    Heading
} from "bumbag"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    getTrans,
    Trans,
    useLocale,
    useTrans
} from "../../i18n"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import {
    Disclosure,
    VerifyDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"
import { usePlayerDepositForm } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { useFormContext } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"
import Separator from "../../layout/Separator"
import { FundReviewStatusLabel } from "../FundReviewStatus"

export const ViewPlayerDepositDisclosure = ({ brandId, formId, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <ViewPlayerDepositDialog
                    brandId={brandId}
                    formId={formId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ViewPlayerDepositDialog = ({ brandId, formId, dialog, ...props }) => {
    let title = useTrans({
        zh: "查看审核",
        en: "View Deposit"
    })

    let { isLoading, data, error } = usePlayerDepositForm(brandId, formId)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} {...dialog} />
    }

    return (
        <Dialog.Modal
            title={title}
            showCloseButton
            hideOnClickOutside={false}
            {...dialog}
        >
            <PlayerDepositFormBody record={data} />
        </Dialog.Modal>
    )
}

export const PlayerDepositFormBody = ({ record }) => {
    return (
        <FieldStack>
            <FieldValues marginLeft="1.5rem">
                <NormalLabel>
                    <Trans zh="用户名" en="Username" />
                </NormalLabel>
                <Text.Block alignY="center">{record?.userName}</Text.Block>

                <NormalLabel>
                    <Trans zh="订单号" en="Order Id" />
                </NormalLabel>
                <Text.Block alignY="center">{record?.id}</Text.Block>

                <NormalLabel>
                    <Trans zh="充值类型 " en="Payment Method" />
                </NormalLabel>
                <Text.Block alignY="center">
                    <PaymentMethodLabel value={record?.paymentMethodId} />
                </Text.Block>

                <NormalLabel>
                    <Trans zh="通道名称" en="Channel Name" />
                </NormalLabel>
                <Text.Block alignY="center">{record?.channelName}</Text.Block>

                <NormalLabel>
                    <Trans zh="充值金额" en="Deposit Amount" />
                </NormalLabel>
                <Text.Block alignY="center">
                    <FormattedNumberDecimal value={record?.amount} />
                </Text.Block>

                <NormalLabel>
                    <Trans zh="申请时间" en="Application Time" />
                </NormalLabel>
                <Text.Block alignY="center">
                    <FormattedDateTime value={record?.createdOn} />
                </Text.Block>

                <NormalLabel>
                    <Trans zh="更新时间" en="Updated Time" />
                </NormalLabel>
                <Text.Block alignY="center">
                    <FormattedDateTime value={record?.updatedOn} />
                </Text.Block>

                <NormalLabel>
                    <Trans zh="状态" en="Status" />
                </NormalLabel>
                <Text.Block alignY="center">
                    <FundReviewStatusLabel value={record?.status} />
                </Text.Block>

                <NormalLabel>
                    <Trans zh="附件" en="Attachment" />
                </NormalLabel>
                <Text.Block alignY="center"></Text.Block>
            </FieldValues>
            <Separator />
            <Heading use="h6">
                <Trans zh="银行信息" en="Bank Info" />
            </Heading>
            <FieldValues marginLeft="1.5rem">
                <NormalLabel>
                    <Trans zh="充值银行" en="Bank" />
                </NormalLabel>
                <Text.Block alignY="center">
                    {record?.applyInfo?.sendBankAccount}
                </Text.Block>

                <NormalLabel>
                    <Trans zh="收款账户名" en="Account Name" />
                </NormalLabel>
                <Text.Block alignY="center">
                    {record?.applyInfo?.receiveAccountName}
                </Text.Block>

                <NormalLabel>
                    <Trans zh="银行卡帐号" en="Account" />
                </NormalLabel>
                <Text.Block alignY="center">
                    {record?.applyInfo?.receiveBankAccount}
                </Text.Block>

                <NormalLabel>
                    <Trans zh="开户地址" en="Account Address" />
                </NormalLabel>
                <Text.Block alignY="center">
                    {record?.applyInfo?.receiveBankAccountAddress}
                </Text.Block>

                <NormalLabel>
                    <Trans zh="附言" en="Memo" />
                </NormalLabel>
                <Text.Block alignY="center">
                    {record?.applyInfo?.memo}
                </Text.Block>
            </FieldValues>
        </FieldStack>
    )
}
