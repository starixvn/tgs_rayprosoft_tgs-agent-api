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
    Link,
    Flex,
    Set,
    Columns
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
import AlertDialog from "../../dialog/Alert"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { useFundWithdrawForm } from "./client"
import Separator from "../../layout/Separator"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"
import { FundReviewStatusLabel, isFundUnderReview } from "../FundReviewStatus"
import { WithdrawMethodSelect } from "./WithdrawMethod"
import {
    PaymentChannelLabel,
    PaymentChannelSelect
} from "../../payment/channel/PaymentChannel"
import { TextareaField } from "../../textarea"
import { BrandPaymentSelect } from "../../system/brandpayments/BrandPayment"
import BrandPaymentTransferType from "../../system/brandpayments/BrandPaymentTransferType"
import clone from "clone-deep"

export const ViewFundWithdrawDisclosure = ({ brandId, formId, ...props }) => {
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
                <ViewFundWithdrawDialog
                    brandId={brandId}
                    formId={formId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ViewFundWithdrawDialog = ({ brandId, formId, dialog, ...props }) => {
    let title = useTrans({
        zh: "查看提款",
        en: "View Withdraw"
    })

    let { isLoading, data, error } = useFundWithdrawForm(brandId, formId)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} {...dialog} />
    }

    return (
        <FormDialog
            title={title}
            defaultValues={clone(data)}
            showCloseButton
            hideOnClickOutside={false}
            {...dialog}
            maxWidth="80vw"
        >
            <FormBody record={data} readonly />
        </FormDialog>
    )
}

export const FormBody = ({ brandId, record, readonly }) => {
    let disabled = readonly
    let underReview = isFundUnderReview(record?.status)

    return (
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
                    <Trans zh="提款类型 " en="Payment Method" />
                </NormalLabel>
                <Box alignY="center">
                    <PaymentMethodLabel value={record?.paymentMethodId} />
                </Box>

                <NormalLabel>
                    <Trans zh="提款金额" en="Withdraw Amount" />
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
            </FieldValues>
            <Separator />
            <Columns orientation="horizontal" alignY="top">
                <Columns.Column>
                    <Heading use="h6">
                        <Trans zh="银行信息" en="Bank Info" />
                    </Heading>
                    <FieldValues marginLeft="1.5rem">
                        <NormalLabel>
                            <Trans zh="出款方式" en="Status" />
                        </NormalLabel>
                        <WithdrawMethodSelect disabled={disabled} />

                        <NormalLabel>
                            <Trans zh="出款渠道" en="Channel" />
                        </NormalLabel>
                        <BrandPaymentSelect
                            name="brandPaymentId"
                            brandId={brandId}
                            transferType={BrandPaymentTransferType.Withdrawal}
                            required
                            disabled={disabled}
                        />
                    </FieldValues>
                </Columns.Column>
                <Columns.Column>
                    <PayeeInfo record={record} />
                </Columns.Column>
            </Columns>

            <Separator />

            <TextareaField
                name="remark"
                label={<Trans zh="备注" en="Remark" />}
                disabled={disabled}
            />
        </FieldStack>
    )
}

const PayeeInfo = ({ record }) => {
    let hasBankInfo = !!record?.bankInfo
    if (!hasBankInfo) {
        return null
    }

    return (
        <>
            <Heading use="h6">
                <Trans zh="收款人信息" en="Payee Info" />
            </Heading>
            <FieldValues marginLeft="1.5rem">
                <NormalLabel>
                    <Trans zh="收款银行" en="Bank" />
                </NormalLabel>
                <PaymentChannelLabel
                    value={record?.bankInfo?.paymentChannelId}
                />

                <NormalLabel>
                    <Trans zh="收款账户名" en="Account Name" />
                </NormalLabel>
                <Box alignY="center">{record?.bankInfo?.detail?.name}</Box>

                <NormalLabel>
                    <Trans zh="银行卡帐号" en="Account" />
                </NormalLabel>
                <Box alignY="center">{record?.bankInfo?.detail?.account}</Box>

                <NormalLabel>
                    <Trans zh="开户地址" en="Account Address" />
                </NormalLabel>
                <Box alignY="center">
                    {record?.bankInfo?.receiveBankAccountAddress}
                </Box>

                <NormalLabel>
                    <Trans zh="附言" en="Memo" />
                </NormalLabel>
                <Box alignY="center">{record?.bankInfo?.memo}</Box>
            </FieldValues>
        </>
    )
}
