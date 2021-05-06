import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Stack,
    FieldStack,
    Box,
    PageWithHeader,
    TopNav,
    DropdownMenu,
    Group,
    Card,
    Dialog,
    Modal,
    Text,
    FieldWrapper,
    InputField,
    Select,
    SelectField
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch,
    useLocation,
    useHistory
} from "react-router-dom"
import BrandHeader from "../Header"
import PlatformDropdownMenu from "../../platform/DropdownMenu"
import LangDropdownMenu from "../../language/DropdownMenu"
import { GameProviderDropdownMenu } from "../../game/provider/DropdownMenu"
import { GameList } from "../../game/List"
import {
    useAddBankInfo,
    useApplyDeposit,
    useBankInfoList,
    useDeleteBankInfo,
    usePaymentList,
    useWithdrawList
} from "../../client/brand/player"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import { toValue } from "../../platform"
import clone from "clone-deep"

export const BankInfoDialog = ({
    brandCode,
    currency,
    username,
    platform,
    onSubmitted,
    dialog
}) => {
    const addBankInfoDialog = Modal.useState()
    const errorDialog = Modal.useState()

    let platformtype = toValue(platform)

    let { data: bankInfoList = [], isLoading, error, mutate } = useBankInfoList(
        {
            brandCode,
            currency,
            username,
            platformtype
        }
    )

    useEffect(() => {
        if (error) {
            errorDialog.show()
        }
    }, [error])

    return (
        <>
            <Dialog.Modal
                title="Bank Info"
                showCloseButton
                hideOnClickOutside={false}
                footer={
                    <Button onClick={addBankInfoDialog.show}>
                        Add Bank Info
                    </Button>
                }
                {...dialog}
            >
                {bankInfoList.map((bankInfo, index) => (
                    <CardForm
                        key={index}
                        title={`${index + 1}`}
                        record={bankInfo}
                        brandCode={brandCode}
                        currency={currency}
                        username={username}
                        platform={platform}
                        platformtype={platformtype}
                        onSubmitted={() => {
                            mutate()
                            onSubmitted?.()
                        }}
                    />
                ))}
            </Dialog.Modal>
            {addBankInfoDialog.visible && (
                <AddBankInfoDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    onSubmitted={() => {
                        mutate()
                        onSubmitted?.()
                    }}
                    dialog={addBankInfoDialog}
                />
            )}
            {error && (
                <Modal.Backdrop zIndex="19900410" {...errorDialog}>
                    <Dialog.Modal
                        title="Error"
                        showCloseButton
                        type="warning"
                        hideBackdrop={true}
                        onClickClose={() => {
                            reset()
                        }}
                        {...errorDialog}
                    >
                        <Text.Block>{JSON.stringify(error)}</Text.Block>
                    </Dialog.Modal>
                </Modal.Backdrop>
            )}
        </>
    )
}

const AddBankInfoDialog = ({
    brandCode,
    currency,
    username,
    platform,
    onSubmitted,
    dialog,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            detail: {}
        }
    })

    let [formData, setSubmit] = useState()
    let platformtype = toValue(platform)

    let { result, error, isLoading, mutate, ...rest } = useAddBankInfo(
        { brandCode, currency, username, platformtype },
        formData
    )

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        setSubmit(undefined)
        mutate(undefined, false)
    }

    useEffect(() => {
        if (result) {
            onSubmitted?.()
            reset()
            dialog.hide()
        }
    }, [result])

    return (
        <>
            <Modal.Backdrop zIndex="19900410" {...dialog}>
                <Dialog.Modal
                    wrap={(children) => (
                        <FormProvider {...formMethods}>
                            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                                {children}
                            </form>
                        </FormProvider>
                    )}
                    title="Add Bank Info"
                    hideBackdrop={true}
                    showCloseButton
                    showActionButtons
                    hideOnClickOutside={false}
                    {...dialog}
                >
                    <FormBody
                        brandCode={brandCode}
                        currency={currency}
                        username={username}
                        platform={platform}
                        platformtype={platformtype}
                    />
                </Dialog.Modal>
            </Modal.Backdrop>
        </>
    )
}

const CardForm = ({
    brandCode,
    currency,
    username,
    platform,
    record,
    onSubmitted,
    ...props
}) => {
    let platformtype = toValue(platform)
    let formMethods = useForm({ defaultValues: clone(record) })
    let { handleSubmit } = formMethods

    let [deleteBankInfoId, setDeleteBankInfoId] = useState()
    let { result: deleteBankInfoResult } = useDeleteBankInfo(
        { brandCode, currency, username, platformtype },
        deleteBankInfoId
    )

    useEffect(() => {
        if (deleteBankInfoResult) {
            onSubmitted?.()
        }
    }, [deleteBankInfoResult])

    return (
        <FormProvider {...formMethods}>
            <Card
                {...props}
                headerAddon={
                    <Set>
                        <Button
                            palette="danger"
                            variant="link"
                            onClick={() => {
                                setDeleteBankInfoId(record.id)
                            }}
                        >
                            Delete
                        </Button>
                    </Set>
                }
            >
                <FormBody
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    platformtype={platformtype}
                />
            </Card>
        </FormProvider>
    )
}

const FormBody = ({
    brandCode,
    currency,
    username,
    platform,
    platformtype
}) => {
    let { register, watch, setValue } = useFormContext()

    let paymentchannelid = watch("paymentchannelid")

    let { isLoading, data: withdrawList = [], error } = useWithdrawList({
        brandCode,
        currency,
        username,
        platformtype
    })

    let banktypeOptions = withdrawList
        ?.filter((item) => item.channelid === paymentchannelid)
        ?.map((item) => ({
            label: item.banktype,
            value: item.banktype
        }))

    let banktype = watch("banktype")
    let isBank = banktype === "Bank"
    let isCrypto = banktype === "Crypto"
    return (
        <>
            <FieldStack>
                <PaymentChannelSelect
                    name="paymentchannelid"
                    placeholder="Payment Channel"
                    setValue={setValue}
                    selectRef={register}
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platformtype={platformtype}
                />
                {banktypeOptions.length > 0 && (
                    <SelectField
                        name="banktype"
                        selectRef={register}
                        options={banktypeOptions}
                        required
                        label="Bank Type"
                    />
                )}
                {isBank && (
                    <>
                        <InputField
                            name="detail.name"
                            inputRef={register}
                            label="Account Name"
                        />
                        <InputField
                            name="detail.account"
                            inputRef={register}
                            label="Account"
                        />
                    </>
                )}
                {isCrypto && (
                    <>
                        <InputField
                            name="detail.address"
                            inputRef={register}
                            label="Address"
                        />
                    </>
                )}
            </FieldStack>
        </>
    )
}

const PaymentChannelSelect = ({
    brandCode,
    currency,
    username,
    platformtype,
    onChange,
    onChannelChange,
    name,
    setValue,
    ...props
}) => {
    let { isLoading, data, error } = useWithdrawList({
        brandCode,
        currency,
        username,
        platformtype
    })

    let options =
        data?.map?.((item) => ({
            label: item.channelname,
            value: item.channelid
        })) ?? []

    return (
        <SelectField
            name={name}
            isLoading={isLoading}
            options={options}
            label="Channel"
            onChange={(event) => {
                onChange?.(event)

                let channelId = event.target.value
                setValue(name, channelId)

                let record = data?.find((item) => {
                    return item.channelid === channelId
                })
                onChannelChange?.(record)
            }}
            {...props}
        />
    )
}
