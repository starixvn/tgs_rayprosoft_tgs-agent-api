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
import { useApplyDeposit, usePaymentList } from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { toValue } from "../../platform"

export const DepositDialog = ({
    brandCode,
    currency,
    username,
    platform,
    dialog
}) => {
    let { register, handleSubmit, watch, setError, errors } = useForm()
    let [formData, setSubmit] = useState()
    let platformtype = toValue(platform)

    let {
        result: paymentResult,
        isLoading: isLoadingPaymentList
    } = usePaymentList({ brandCode, currency, username, platformtype })

    let { result, error, isLoading, mutate, ...rest } = useApplyDeposit(
        { brandCode, currency, username, platformtype },
        formData
    )

    const onSubmit = handleSubmit(setSubmit)

    const reset = () => {
        setSubmit(undefined)
        mutate(undefined, false)
    }

    useEffect(() => {
        if (result) {
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

    let paymentOptions =
        paymentResult?.data?.map?.(({ channelName, id }) => {
            return {
                label: channelName,
                value: id
            }
        }) ?? []

    return (
        <>
            <Dialog.Modal
                title={username}
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
                    <Group>
                        <SelectField
                            name="paymentid"
                            isLoading={isLoadingPaymentList}
                            options={paymentOptions}
                            selectRef={register}
                        />
                        {/* <InputField
                            name="paymentid"
                            inputRef={register}
                            label="Payment ID"
                            defaultValue="4f6e04c8-baba-461c-a8aa-08d903d6d2f2"
                        /> */}
                    </Group>
                    <InputField
                        name="account"
                        inputRef={register}
                        label="Account"
                        defaultValue="default_bank_account"
                    />
                    <InputField
                        name="amount"
                        type="number"
                        inputRef={register}
                        label="Amount"
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
