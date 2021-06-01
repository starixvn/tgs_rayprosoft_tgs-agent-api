import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    useModalState,
    Dialog,
    Card,
    FieldWrapper,
    Columns,
    DropdownMenu
} from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId
} from "../../user/context"
import Disclosure, {
    AddDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller,
    useWatch
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import {
    useAddDepositBrandPayment,
    useAddWithdrawBrandPayment,
    useUpdateBrandPayment
} from "../../system/brandpayments/client"
import BrandPaymentStatus, {
    BrandPaymentStatusSelect
} from "../../system/brandpayments/BrandPaymentStatus"
import { PaymentProviderSelect } from "../../payment/PaymentProvider"
import { PaymentMethodSelect } from "../../payment/method/PaymentMethod"
import { PaymentChannelSelect } from "../../payment/channel/PaymentChannel"
import { usePaymentMethodChannelList } from "../../payment/client"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { PlayerListCondition } from "../../system/brandpayments/condition"
import Select from "../../select/Select"
import { JsonEditor } from "../../text/editor/JsonEditor"
import NumberRangeInputGroup from "../../number/RangeInput"
import { parseJSON, stringifyJSON } from "../../text/json"
import { Update } from "./menu"

export const EditBrandPaymentDisclosure = ({ record, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
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

export const AddBrandPaymentDisclosure = ({ brandId, onSubmitted }) => {
    let depositDialog = useModalState()
    let withdrawDialog = useModalState()

    let depositLabel = useTrans({
        zh: "新增存款支付",
        en: "Add Deposit Payment"
    })
    let withdrawLabel = useTrans({
        zh: "新增提款支付",
        en: "Add Withdraw Payment"
    })

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <DropdownMenu
                menu={
                    <>
                        <DropdownMenu.Item onClick={depositDialog.show}>
                            {depositLabel}
                        </DropdownMenu.Item>
                        <DropdownMenu.Item onClick={withdrawDialog.show}>
                            {withdrawLabel}
                        </DropdownMenu.Item>
                    </>
                }
            >
                <Button iconAfter="chevron-down">
                    <Trans zh="品牌支付" en="Brand Payment" />
                </Button>
            </DropdownMenu>
            {depositDialog.visible && (
                <AddDepositDialog
                    title={depositLabel}
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={depositDialog}
                />
            )}
            {withdrawDialog.visible && (
                <AddWithdrawDialog
                    title={withdrawLabel}
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={withdrawDialog}
                />
            )}
        </>
    )
}

const EditDialog = ({
    record: { id, extendedData = {}, ...record },
    onSubmitted,
    dialog
}) => {
    let title = useTrans({
        zh: "Edit",
        en: "Edit"
    })

    let [formData, setSubmit] = useState()
    let { isLoading, result, error, deleteCache, mutate } =
        useUpdateBrandPayment(id, formData)

    const onSubmit = ({ extendedData, ...data }) => {
        setSubmit({
            ...record,
            ...data,
            extendedData: parseJSON(extendedData)
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                ...clone(record),
                extendedData: stringifyJSON(extendedData)
            }}
            onSubmit={onSubmit}
            onSubmitted={onSubmitted}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            title={title}
            dialog={dialog}
            width="65vw"
            maxWidth="80vw"
        >
            <EditFormBody brandId={record?.brandId} />
        </FormDialog>
    )
}

const EditFormBody = () => {
    return <ExtendedDataField />
}

const AddDepositDialog = ({ title, brandId, dialog }) => {
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, deleteCache, mutate } =
        useAddDepositBrandPayment(formData)

    const onSubmit = (data) => {
        setSubmit({
            brandId,
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                paymentStatus: BrandPaymentStatus.Enabled,
                playerListCondition: PlayerListCondition.MatchOne,
                allPlayers: true,
                transferLimitFrom: "0",
                depositedAmountMin: "0",
                depositedAmountUnlimited: true,
                extendedData: JSON.stringify({})
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            title={title}
            dialog={dialog}
            width="65vw"
            maxWidth="80vw"
        >
            <FormBody brandId={brandId} />
        </FormDialog>
    )
}

const AddWithdrawDialog = ({ title, brandId, dialog }) => {
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, deleteCache, mutate } =
        useAddWithdrawBrandPayment(formData)

    const onSubmit = (data) => {
        setSubmit({
            brandId,
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                paymentStatus: BrandPaymentStatus.Enabled,
                playerListCondition: PlayerListCondition.MatchOne,
                allPlayers: true,
                transferLimitFrom: "0",
                depositedAmountMin: "0",
                depositedAmountUnlimited: true,
                extendedData: JSON.stringify({})
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            title={title}
            dialog={dialog}
            width="65vw"
            maxWidth="80vw"
        >
            <FormBody brandId={brandId} />
        </FormDialog>
    )
}

const FormBody = ({ brandId }) => {
    return (
        <FieldStack>
            <FieldStack orientation="horizontal">
                {!brandId && (
                    <FieldWrapper label="Brand">
                        <MerchantIdSelect />
                    </FieldWrapper>
                )}
            </FieldStack>

            <Columns>
                <Columns.Column>
                    <FieldStack>
                        <FieldStack orientation="horizontal">
                            <InputField
                                name="channelName"
                                label="Channel Name"
                                flex="2"
                            />
                            <FieldWrapper label="Status">
                                <BrandPaymentStatusSelect name="paymentStatus" />
                            </FieldWrapper>
                        </FieldStack>

                        <PaymentMethodChannelIdSelect />

                        <TransferLimitRangeField />
                    </FieldStack>
                </Columns.Column>

                <Columns.Column>
                    <ExtendedDataField />
                </Columns.Column>
            </Columns>
        </FieldStack>
    )
}

const TransferLimitRangeField = () => {
    let { control } = useFormContext()

    let TransferLimitFromFieldName = "transferLimitFrom"
    let transferLimitFrom = useWatch({
        control,
        name: TransferLimitFromFieldName
    })

    let TransferLimitToFieldName = "transferLimitTo"
    let transferLimitTo = useWatch({
        control,
        name: TransferLimitToFieldName
    })
    return (
        <FieldWrapper label={<Trans zh="单笔限额" en="Limit" />}>
            <NumberRangeInputGroup
                names={[TransferLimitFromFieldName, TransferLimitToFieldName]}
                required
                min={[0, transferLimitFrom]}
                max={[transferLimitTo]}
                width="100%"
            />
        </FieldWrapper>
    )
}

const ExtendedDataField = () => {
    let FieldName = "extendedData"
    return (
        <FieldWrapper label="Extended Data">
            <JsonEditor name={FieldName} required height="30vh" />
        </FieldWrapper>
    )
}

const PaymentMethodChannelIdSelect = ({}) => {
    let { isLoading, data, error } = usePaymentMethodChannelList()

    let { control, setValue } = useFormContext()
    let PaymentIdFieldName = "paymentId"
    let paymentId = useWatch({
        control,
        name: PaymentIdFieldName
    })

    let MethodIdFieldName = "methodId"
    let methodId = useWatch({
        control,
        name: MethodIdFieldName
    })

    let ChannelIdFieldName = "channelId"
    let channelId = useWatch({
        control,
        name: ChannelIdFieldName
    })

    let methodIdRange = data?.allData
        ?.filter((option) => {
            let hasPayment = !!paymentId
            if (hasPayment && option.paymentId != paymentId) {
                return false
            }

            return true
        })
        .map(({ methodId }) => methodId)

    let channelIdRange = data?.allData
        ?.filter((option) => {
            let hasPayment = !!paymentId
            if (hasPayment && option.paymentId != paymentId) {
                return false
            }

            let hasMethod = !!methodId
            if (hasMethod && option.methodId != methodId) {
                return false
            }
            return true
        })
        .map(({ channelId }) => channelId)

    const updatePaymentMethodChannel = () => {
        let item = data?.allData?.find((item) => {
            return (
                item.paymentId === paymentId &&
                item.methodId === methodId &&
                item.channelId === channelId
            )
        })
        setValue("paymentMethodChannelId", item?.id)
    }

    useEffect(() => {
        updatePaymentMethodChannel()
    }, [paymentId, methodId, channelId])

    return (
        <FieldWrapper label="Payment Method Channel">
            <Group>
                <PaymentProviderSelect
                    name={PaymentIdFieldName}
                    required
                    isLoading={isLoading}
                    onChange={(event) => {
                        setValue(MethodIdFieldName, "")
                        setValue(ChannelIdFieldName, "")
                    }}
                />
                <PaymentMethodSelect
                    name={MethodIdFieldName}
                    required
                    isLoading={isLoading}
                    range={methodIdRange}
                    onChange={(event) => {
                        setValue(ChannelIdFieldName, "")
                    }}
                />
                <PaymentChannelSelect
                    name="channelId"
                    required
                    isLoading={isLoading}
                    range={channelIdRange}
                    onChange={() => {}}
                />
                <Input type="hidden" name="paymentMethodChannelId" />
            </Group>
        </FieldWrapper>
    )
}
