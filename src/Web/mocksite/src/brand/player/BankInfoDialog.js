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
    usePaymentList
} from "../../client/brand/player"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import { toValue } from "../../platform"

export const BankInfoDialog = ({
    brandCode,
    currency,
    username,
    platform,
    onSubmitted,
    dialog
}) => {
    const addBankInfoDialog = Modal.useState()
    const editBankInfoDialog = Modal.useState()
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

    let [record, setRecord] = useState()

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
                    <Card title={`${index + 1}`}>
                        TODO: {JSON.stringify(bankInfo)}
                    </Card>
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
            {editBankInfoDialog.visible && (
                <EditBankInfoDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    record={record}
                    onSubmitted={() => {
                        mutate()
                        onSubmitted?.()
                    }}
                    dialog={editBankInfoDialog}
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
    let { register } = formMethods

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

const EditBankInfoDialog = ({ record, onSubmitted, dialog, ...props }) => {
    let formMethods = useForm()

    const onSubmit = (data) => {
        console.log("onSubmit", data)
    }

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
                    title="Edit Bank Info"
                    hideBackdrop={true}
                    showCloseButton
                    hideOnClickOutside={false}
                    {...dialog}
                >
                    {JSON.stringify(record)}
                </Dialog.Modal>
            </Modal.Backdrop>
        </>
    )
}

const CardForm = ({ record, onSubmitted }) => {}

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

    return <Select options={options} {...props} />
}
