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
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Disclosure, DeleteDisclosure } from "../../dialog/Disclosure"
import {
    useAddPaymentMethodChannel,
    useDeletePaymentMethodChannel,
    usePaymentMethodChannelList
} from "../../payment/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import {
    PaymentProviderLabel,
    PaymentProviderSelect
} from "../../payment/PaymentProvider"
import {
    PaymentMethodLabel,
    PaymentMethodSelect
} from "../../payment/method/PaymentMethod"
import {
    PaymentChannelLabel,
    PaymentChannelSelect
} from "../../payment/channel/PaymentChannel"
import { Trans, useTrans } from "../../i18n"
import { FormDialog } from "../../form/FormDialog"
import AlertDialog from "../../dialog/Alert"
import Backdrop from "../../dialog/Backdrop"
import { DeleteText } from "../../button/ActionButtons"
import { View, Update } from "./menu"

export const PaymentMethodChannelListDisclosure = ({}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="支付列表" en="Payment  List" />
            </Disclosure>
            {dialog.visible && <ListDialog dialog={dialog} />}
        </>
    )
}

const ListDialog = ({ dialog }) => {
    let { isLoading, data, error, mutate, deleteCache } =
        usePaymentMethodChannelList()

    let items = data?.allData
    let columns = useColumns({ items, mutate })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    return (
        <Dialog.Modal
            title="Payment Method Channel List"
            showCloseButton
            {...dialog}
            hideOnClickOutside={false}
            width="60vw"
            maxWidth="60vw"
        >
            <FieldStack>
                <ActionBar mutate={mutate} />
                <Table columns={columns} data={items} />
            </FieldStack>
        </Dialog.Modal>
    )
}

const ActionBar = ({ mutate }) => {
    return (
        <Box alignX="right">
            <AddDialogDisclosure onSubmitted={mutate} />
        </Box>
    )
}

const AddDialogDisclosure = ({ onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans
                    zh="Add Payment Method Channel"
                    en="Add Payment Method Channel"
                />
            </Disclosure>
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "Add Payment Method Channel",
        en: "Add Payment Method Channel"
    })
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useAddPaymentMethodChannel(formData)

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                onSubmit={onSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onSubmitted={onSubmitted}
                onCloseErrorAlert={reset}
                title={title}
                dialog={dialog}
                width="40vw"
            >
                <FormBody />
            </FormDialog>
        </Backdrop>
    )
}

const FormBody = () => {
    return (
        <FieldStack orientation="horizontal">
            <FieldWrapper label="Payment Provider">
                <PaymentProviderSelect name="paymentId" required />
            </FieldWrapper>
            <FieldWrapper label="Payment Method">
                <PaymentMethodSelect name="methodId" required />
            </FieldWrapper>
            <FieldWrapper label="Payment Channel">
                <PaymentChannelSelect name="channelId" required />
            </FieldWrapper>
        </FieldStack>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "paymentId",
                Header: <Trans zh="支付厂商" en="Payment Name" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentProviderLabel value={value} />
            }),
            textColumn({
                accessor: "methodId",
                Header: <Trans zh="支付方式" en="Payment Method" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentMethodLabel value={value} />
            }),
            textColumn({
                accessor: "channelId",
                Header: <Trans zh="支付通道" en="Payment Channel" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentChannelLabel value={value} />
            })
        ]

        if (hasUpdateAuth) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    Header: <Trans zh="Actions" en="Actions" />,
                    disableSortBy: true,
                    Cell: ({ value }) => (
                        <DeleteDialogDisclosure
                            record={value}
                            onSubmitted={mutate}
                        />
                    )
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}

const DeleteDialogDisclosure = ({ record, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <DeleteDisclosure {...dialog} />
            {dialog.visible && (
                <DeleteDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const DeleteDialog = ({ record, onSubmitted, dialog }) => {
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useDeletePaymentMethodChannel(formData)

    const onSubmit = (data) => setSubmit(record.id)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                onSubmit={onSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onSubmitted={onSubmitted}
                onCloseErrorAlert={reset}
                type="danger"
                title="Confirm Delete?"
                submitText={<DeleteText />}
                isLoading={isLoading}
                result={result}
                error={error}
                dialog={dialog}
                hideBackdrop
                width="30vw"
                actionButtonsProps={{
                    submitProps: {
                        palette: "danger",
                        variant: "outlined"
                    }
                }}
            >
                <FieldStack orientation="horizontal">
                    <FieldWrapper label="Payment">
                        <PaymentProviderLabel value={record.paymentId} />
                    </FieldWrapper>

                    <FieldWrapper label="Method">
                        <PaymentMethodLabel value={record.methodId} />
                    </FieldWrapper>

                    <FieldWrapper label="Channel">
                        <PaymentChannelLabel value={record.channelId} />
                    </FieldWrapper>
                </FieldStack>
            </FormDialog>
        </Backdrop>
    )
}
