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
    useApplyWithdraw,
    useBankInfoList,
    useDeleteBankInfo,
    usePaymentList
} from "../../client/brand/player"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import { toValue } from "../../platform"
import clone from "clone-deep"

export const WithdrawDialog = ({
    brandCode,
    currency,
    username,
    platform,
    onSubmitted,
    dialog
}) => {
    let platformtype = toValue(platform)
    let formMethods = useForm()
    let { register } = formMethods

    let {
        data: bankInfoList = [],
        isLoading: isLoadingBankInfo
    } = useBankInfoList({
        brandCode,
        currency,
        username,
        platformtype
    })
    let [formData, setSubmit] = useState()
    let { result, error, isLoading, mutate, ...rest } = useApplyWithdraw(
        { brandCode, currency, username, platformtype },
        formData
    )

    const onSubmit = formMethods.handleSubmit(setSubmit)

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

    const errorDialog = Modal.useState()
    useEffect(() => {
        if (error) {
            errorDialog.show()
        }
    }, [error])

    let cardOptions = bankInfoList?.map((bankInfo) => ({
        label: bankInfo.id,
        value: bankInfo.id
    }))

    return (
        <>
            <Dialog.Modal
                title="Withdraw"
                showCloseButton
                showActionButtons
                hideOnClickOutside={false}
                actionButtonsProps={{
                    isLoading,
                    onClickSubmit: onSubmit
                }}
                {...dialog}
            >
                <FieldStack>
                    <SelectField
                        name="bankinfoid"
                        options={cardOptions}
                        selectRef={register}
                        label="Bank"
                        required
                    />

                    <InputField
                        name="amount"
                        type="number"
                        inputRef={register}
                        label="Amount"
                        required
                    />

                    <InputField
                        name="paypassword"
                        inputRef={register}
                        label="Pay Password"
                        required
                    />
                </FieldStack>
            </Dialog.Modal>
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
