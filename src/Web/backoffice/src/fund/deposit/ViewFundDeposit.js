import React from "react"
import {
    Spinner,
    useModalState,
    FieldStack,
    Box,
    Text,
    Dialog,
    Modal,
    Heading,
    Link
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
import { Update, View } from "./menu"
import {
    Disclosure,
    VerifyDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"
import { useFundDepositForm } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { useFormContext } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"
import Separator from "../../layout/Separator"
import { FundReviewStatusLabel } from "../FundReviewStatus"
import Textarea, { TextareaField } from "../../textarea"
import Input from "../../input/Input"

export const ViewFundDepositDisclosure = ({ brandId, formId, ...props }) => {
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
                <ViewFundDepositDialog
                    brandId={brandId}
                    formId={formId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ViewFundDepositDialog = ({ brandId, formId, dialog, ...props }) => {
    let title = useTrans({
        zh: "查看充值",
        en: "View Deposit"
    })

    let { isLoading, data, error } = useFundDepositForm(brandId, formId)

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
            <FormBody record={data} />
        </Dialog.Modal>
    )
}

export const FormBody = ({ brandId, record, readonly }) => {
    let hasAttachment = !!record?.paymentReturn?.manualReceiptFileId
    let attachmentUrl = `/api/fs/${record?.paymentReturn?.manualReceiptFileId}`

    let disabled = readonly
    return (
        <>
            <FieldStack>
                <FieldValues marginLeft="1.5rem">
                    <NormalLabel>
                        <Trans zh="用户名" en="Username" />
                    </NormalLabel>
                    <Box alignY="center">{record?.userName}</Box>

                    <NormalLabel>
                        <Trans zh="订单号" en="Order Id" />
                    </NormalLabel>
                    <Box alignY="center">{record?.id}</Box>

                    <NormalLabel>
                        <Trans zh="充值类型 " en="Payment Method" />
                    </NormalLabel>
                    <Box alignY="center">
                        <PaymentMethodLabel value={record?.paymentMethodId} />
                    </Box>

                    <NormalLabel>
                        <Trans zh="通道名称" en="Channel Name" />
                    </NormalLabel>
                    <Box alignY="center">{record?.channelName}</Box>

                    <NormalLabel>
                        <Trans zh="充值金额" en="Deposit Amount" />
                    </NormalLabel>
                    <Box alignY="center">
                        <FormattedNumberDecimal value={record?.amount} />
                    </Box>

                    <NormalLabel>
                        <Trans zh="申请时间" en="Application Time" />
                    </NormalLabel>
                    <Box alignY="center">
                        <FormattedDateTime value={record?.createdOn} />
                    </Box>

                    <NormalLabel>
                        <Trans zh="更新时间" en="Updated Time" />
                    </NormalLabel>
                    <Box alignY="center">
                        <FormattedDateTime
                            value={record?.updatedOn}
                            placeholder="-"
                        />
                    </Box>

                    <NormalLabel>
                        <Trans zh="状态" en="Status" />
                    </NormalLabel>
                    <Box alignY="center">
                        <FundReviewStatusLabel value={record?.status} />
                    </Box>

                    {hasAttachment && (
                        <>
                            <NormalLabel>
                                <Trans zh="附件" en="Attachment" />
                            </NormalLabel>
                            <Box alignY="center">
                                <Link href={attachmentUrl} target="_blank">
                                    <Trans zh="连结" en="Link" />
                                </Link>
                            </Box>
                        </>
                    )}
                </FieldValues>
                <Separator />
                <BankInfoBox record={record} />
                <Separator />
                <TextareaField
                    name="remark"
                    label={<Trans zh="备注" en="Remark" />}
                    disabled={disabled}
                />
            </FieldStack>
            <Input type="hidden" value={brandId} />
        </>
    )
}

const BankInfoBox = ({ record }) => {
    let hasBankInfo = !!record?.applyInfo?.sendBankAccount
    if (!hasBankInfo) {
        return null
    }

    let hasBankAddress = !!record?.applyInfo?.receiveBankAccountAddress
    let hasMemo = !!record?.applyInfo?.memo
    return (
        <>
            <Heading use="h6">
                <Trans zh="银行信息" en="Bank Info" />
            </Heading>
            <FieldValues marginLeft="1.5rem">
                <NormalLabel>
                    <Trans zh="充值账户" en="Deposit Bank Account" />
                </NormalLabel>
                <Box alignY="center">{record?.applyInfo?.sendBankAccount}</Box>

                <NormalLabel>
                    <Trans zh="收款账户名" en="Account Name" />
                </NormalLabel>
                <Box alignY="center">
                    {record?.applyInfo?.receiveAccountName}
                </Box>

                <NormalLabel>
                    <Trans zh="银行卡帐号" en="Account" />
                </NormalLabel>
                <Box alignY="center">
                    {record?.applyInfo?.receiveBankAccount}
                </Box>

                {hasBankAddress && (
                    <>
                        <NormalLabel>
                            <Trans zh="开户地址" en="Account Address" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.applyInfo?.receiveBankAccountAddress}
                        </Box>
                    </>
                )}

                {hasMemo && (
                    <>
                        <NormalLabel>
                            <Trans zh="附言" en="Memo" />
                        </NormalLabel>
                        <Box alignY="center">{record?.applyInfo?.memo}</Box>
                    </>
                )}
            </FieldValues>
        </>
    )
}
