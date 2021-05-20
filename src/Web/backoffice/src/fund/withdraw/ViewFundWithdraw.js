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
import {
    FundReviewStatusLabel,
    isFundApproved,
    isFundUnderReview
} from "../FundReviewStatus"
import { WithdrawMethodSelect } from "./WithdrawMethod"
import {
    PaymentChannelLabel,
    PaymentChannelSelect
} from "../../payment/channel/PaymentChannel"
import { TextareaField } from "../../textarea"
import { BrandPaymentSelect } from "../../system/brandpayments/BrandPayment"
import BrandPaymentTransferType from "../../system/brandpayments/BrandPaymentTransferType"
import clone from "clone-deep"
import { useFormContext } from "react-hook-form"
import {
    useBrandPayment,
    useBrandPaymentList
} from "../../system/brandpayments/client"
import { parseJSON } from "../../text/json"
import Input from "../../input/Input"

export const ViewFundWithdrawDisclosure = ({ brandId, formId, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

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
            <FormBody brandId={brandId} record={data} readonly />
        </FormDialog>
    )
}

export const FormBody = ({ brandId, record, readonly }) => {
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
                        <Trans zh="提款类型 " en="Payment Channel" />
                    </NormalLabel>
                    <Box alignY="center">
                        <PaymentChannelLabel
                            value={record?.bankInfo?.paymentChannelId}
                        />
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
                        <PayerInfo
                            brandId={brandId}
                            record={record}
                            disabled={disabled}
                            readonly={readonly}
                        />
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
            <Input type="hidden" value={brandId} />
        </>
    )
}

const PayerInfo = ({ brandId, record, disabled }) => {
    let { watch } = useFormContext()

    let underReview = isFundUnderReview(record?.status)
    let isApproved = isFundApproved(record?.status)

    let {
        isLoading,
        data: brandPayment,
        error
    } = useBrandPayment(
        record?.brandPaymentId,
        isApproved
            ? {
                  brandId,
                  transferType: BrandPaymentTransferType.Withdrawal,
                  showAll: true
              }
            : undefined
    )

    let detail = parseJSON(brandPayment?.extendedData)
    return (
        <>
            <Heading use="h6">
                <Trans zh="出款信息" en="Payout Info" />
            </Heading>
            <FieldValues marginLeft="1.5rem">
                {underReview && (
                    <>
                        <NormalLabel>
                            <Trans zh="出款方式" en="Status" />
                        </NormalLabel>
                        <WithdrawMethodSelect disabled={disabled} />
                    </>
                )}

                <NormalLabel>
                    <Trans zh="出款渠道" en="Withdrawal Payment" />
                </NormalLabel>
                <Box alignY="center">
                    <BrandPaymentSelect
                        name="brandPaymentId"
                        brandId={brandId}
                        transferType={BrandPaymentTransferType.Withdrawal}
                        paymentChannelId={record?.bankInfo?.paymentChannelId}
                        required
                        disabled={disabled}
                    />
                </Box>

                {isApproved && brandPayment && (
                    <>
                        <NormalLabel>
                            <Trans zh="出款通道" en="Channel" />
                        </NormalLabel>
                        <Box alignY="center">
                            <PaymentChannelLabel
                                value={brandPayment?.paymentChannelId}
                            />
                        </Box>

                        {detail?.name && (
                            <>
                                <NormalLabel>
                                    <Trans zh="帐号名称" en="Account Name" />
                                </NormalLabel>
                                <Box alignY="center">{detail?.name}</Box>
                            </>
                        )}

                        {detail?.bankAccount && (
                            <>
                                <NormalLabel>
                                    <Trans zh="银行帐号" en="Account" />
                                </NormalLabel>
                                <Box alignY="center">{detail?.bankAccount}</Box>
                            </>
                        )}
                    </>
                )}
            </FieldValues>
        </>
    )
}

const PayeeInfo = ({ record }) => {
    let hasBankInfo = !!record?.bankInfo
    if (!hasBankInfo) {
        return null
    }

    let hasBankName = !!record?.bankInfo?.detail?.bankName
    let hasBankBranch = !!record?.bankInfo?.detail?.bankBranch

    let hasAccountName = !!record?.bankInfo?.detail?.name
    let hasBankAccount = !!record?.bankInfo?.detail?.bankAccount
    let hasMemo = !!record?.bankInfo?.detail?.memo
    let isBank = hasAccountName && hasBankAccount

    let hasAddress = !!record?.bankInfo?.detail?.address
    let isCrypto = hasAddress
    return (
        <>
            <Heading use="h6">
                <Trans zh="收款人信息" en="Payee Info" />
            </Heading>
            <FieldValues marginLeft="1.5rem">
                <NormalLabel>
                    <Trans zh="收款通道" en="Channel" />
                </NormalLabel>
                <Box alignY="center">
                    <PaymentChannelLabel
                        value={record?.bankInfo?.paymentChannelId}
                    />
                </Box>

                {hasBankName && (
                    <>
                        <NormalLabel>
                            <Trans zh="银行" en="Bank" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.bankInfo?.detail?.bankName}
                        </Box>
                    </>
                )}

                {hasBankBranch && (
                    <>
                        <NormalLabel>
                            <Trans zh="银行分行" en="Bank Branch" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.bankInfo?.detail?.bankBranch}
                        </Box>
                    </>
                )}

                {hasAccountName && (
                    <>
                        <NormalLabel>
                            <Trans zh="收款账户名" en="Account Name" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.bankInfo?.detail?.name}
                        </Box>
                    </>
                )}

                {hasBankAccount && (
                    <>
                        <NormalLabel>
                            <Trans zh="银行卡帐号" en="Account" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.bankInfo?.detail?.bankAccount}
                        </Box>
                    </>
                )}

                {hasAddress && (
                    <>
                        <>
                            <NormalLabel>
                                <Trans zh="钱包地址" en="Wallet address" />
                            </NormalLabel>
                            <Box alignY="center">
                                {record?.bankInfo?.detail?.address}
                            </Box>
                        </>
                    </>
                )}

                {hasMemo && (
                    <>
                        <NormalLabel>
                            <Trans zh="附言" en="Memo" />
                        </NormalLabel>
                        <Box alignY="center">
                            {record?.bankInfo?.detail?.memo}
                        </Box>
                    </>
                )}
            </FieldValues>
        </>
    )
}
