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
    Columns
} from "bumbag"
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
import {
    useAddGameProviderGameList,
    useUpdateGameProviderGame
} from "../../game/games/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import GameStatus, { GameStatusSelectField } from "../../game/games/GameStatus"
import { GameTypeSelectField } from "../../game/GameType"
import {
    GamePlatformLabel,
    GamePlatformTypeSelectField
} from "../../game/games/GamePlatform"
import { useDropzone } from "react-dropzone"
import { useAddBrandPayment } from "../../system/brandpayments/client"
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
import { BrandPaymentTransferTypeSelectField } from "../../system/brandpayments/BrandPaymentTransferType"

export const AddBrandPaymentDisclosure = ({ brandId, onSubmitted }) => {
    let dialog = useModalState()

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddDialog = ({ brandId, dialog }) => {
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, deleteCache, mutate } =
        useAddBrandPayment(formData)

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
            title="Add"
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

                <BrandPaymentTransferTypeSelectField
                    name="transferType"
                    label="Transfer Type"
                    required
                />
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
    return (
        <FieldWrapper label="Extended Data">
            <JsonEditor name="extendedData" required height="30vh" />
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
