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
    usePaymentList
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
                    <FormBody />
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
        <FormProvider>
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
                <FormBody />
            </Card>
        </FormProvider>
    )
}

const FormBody = ({}) => {
    let { register } = useFormContext()
    return (
        <>
            <FieldStack>
                <InputField
                    name="paymentchannelid"
                    inputRef={register}
                    value="e761a2d3-5705-417f-8d97-43cb7aea0142"
                    label="Channel"
                />
                <BankTypeSelect name="banktype" selectRef={register} />
                <InputField name="detail.name" inputRef={register} />
                <InputField name="detail.account" inputRef={register} />
            </FieldStack>
        </>
    )
}

const BankTypeSelect = ({ ...props }) => {
    let options = [
        {
            label: "Bank",
            value: "Bank"
        },
        {
            label: "Crypto",
            value: "Crypto"
        }
    ]

    return <SelectField options={options} label="Type" {...props} />
}
