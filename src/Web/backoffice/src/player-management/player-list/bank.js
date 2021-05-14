import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Icon,
    Spinner,
    Stack,
    Flex,
    Box,
    Card as _Card,
    Set,
    Button,
    Text,
    Label,
    Dialog,
    Table,
    useClassName,
    FieldStack
} from "bumbag"
import { FieldValues, TwoColumns } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber } from "../../i18n"
import { faMinus, faCog, faPlus } from "@fortawesome/free-solid-svg-icons"
import {
    useEditPlayerBank,
    useDeletePlayerBank,
    useAddPlayerBank
} from "./client"
import AlertDialog from "../../dialog/Alert"
import Backdrop from "../../dialog/Backdrop"
import { Tab as _Tab, TabPanel } from "../../tab"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { CancelText, SubmitText } from "../../button/ActionButtons"
import Input from "../../input/Input"
import FormDialog from "../../form/FormDialog"
import {
    PaymentChannelLabel,
    PaymentChannelSelect
} from "../../payment/channel/PaymentChannel"
import { BrandPaymentMethodBankTypeSelect } from "../../payment/method/BrandPaymentMethodBankType"
import { useFormContext } from "react-hook-form"
import PaymentMethodBankType from "../../payment/method/PaymentMethodBankType"
import { useBrandPaymentList } from "../../system/brandpayments/client"
import BrandPaymentTransferType from "../../system/brandpayments/BrandPaymentTransferType"
import clone from "clone-deep"

export const Key = "banks"

export const Tab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="银行卡信息" en="Bank" />
        </_Tab>
    )
}

export const Panel = ({
    brandId,
    playerCode,
    banks = [],
    edit = false,
    onSubmitted,
    ...props
}) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <TwoColumns>
                {banks.map((bank, index) => (
                    <Bank
                        key={index}
                        brandId={brandId}
                        playerCode={playerCode}
                        record={bank}
                        onSubmitted={onSubmitted}
                        edit={edit}
                    />
                ))}
            </TwoColumns>
            {edit && (
                <AddBank
                    brandId={brandId}
                    playerCode={playerCode}
                    onSubmitted={onSubmitted}
                />
            )}
        </TabPanel>
    )
}

const Bank = ({
    brandId,
    playerCode,
    record = {},
    edit = false,
    onSubmitted
}) => {
    let { bankType = "" } = record
    let deleteDialog = useModalState()
    let editDialog = useModalState()

    let isBank = PaymentMethodBankType.isBank(bankType)
    let isCrypto = PaymentMethodBankType.isCrypto(bankType)
    return (
        <Card>
            {edit && (
                <Set alignX="right">
                    <Disclosure
                        palette="danger"
                        variant="outlined"
                        {...deleteDialog}
                    >
                        <Icon
                            icon={faMinus}
                            fontSize="300"
                            type="font-awesome"
                        />
                    </Disclosure>
                    {deleteDialog.visible && (
                        <DeleteDialog
                            playerCode={playerCode}
                            bank={record}
                            onSubmitted={onSubmitted}
                            dialog={deleteDialog}
                        />
                    )}
                    <Disclosure {...editDialog}>
                        <Icon icon={faCog} fontSize="300" type="font-awesome" />
                    </Disclosure>
                    {editDialog.visible && (
                        <EditDialog
                            brandId={brandId}
                            playerCode={playerCode}
                            record={record}
                            onSubmitted={onSubmitted}
                            dialog={editDialog}
                        />
                    )}
                </Set>
            )}
            <FieldValues>
                <Box alignY="center">
                    <PaymentChannelTrans />
                </Box>
                <PaymentChannelLabel value={record?.paymentChannelId} />

                {isBank && (
                    <>
                        <Box alignY="center">
                            <BankLabel />
                        </Box>
                        <Box alignY="center">{record?.detail?.name ?? ""}</Box>

                        <Box alignY="center">
                            <BankAccount />
                        </Box>
                        <Box alignY="center">
                            {record?.detail?.account ?? ""}
                        </Box>
                    </>
                )}

                {isCrypto && (
                    <>
                        <Box alignY="center">
                            <Trans zh="钱包地址" en="Wallet address" />
                        </Box>
                        <Box alignY="center">
                            {record?.detail?.address ?? ""}
                        </Box>
                    </>
                )}
            </FieldValues>
        </Card>
    )
}

const AddBank = ({ brandId, playerCode, onSubmitted }) => {
    let dialog = useModalState()
    return (
        <>
            <TwoColumns>
                <Card alignX="center">
                    <Disclosure {...dialog}>
                        <Icon
                            icon={faPlus}
                            fontSize="300"
                            type="font-awesome"
                        />
                    </Disclosure>
                </Card>
            </TwoColumns>
            {dialog.visible && (
                <AddDialog
                    brandId={brandId}
                    playerCode={playerCode}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const Card = ({ children, ...props }) => (
    <_Card
        variant="bordered"
        margin="major-1"
        css={css`
            flex: 1 50%;
        `}
        {...props}
    >
        {children}
    </_Card>
)

const PaymentBankTypeTrans = () => <Trans zh="支付类型" en="Payment Type" />

const PaymentChannelTrans = () => <Trans zh="支付通道" en="Payment Channel" />

const RealnameLabel = () => <Trans zh="真实姓名" en="Real name" />

const BankLabel = () => <Trans zh="帐号名称" en="Account Name" />

const BankAccount = () => <Trans zh="银行卡号" en="Bank Account" />

const AddDialog = ({ brandId, playerCode, onSubmitted, dialog }) => {
    let [submitData, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } = useAddPlayerBank(
        playerCode,
        submitData
    )

    const onSubmit = (data = {}) => {
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
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={{
                    detail: {}
                }}
                onSubmit={onSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
            >
                <EditBody brandId={brandId} />
            </FormDialog>
        </Backdrop>
    )
}

const EditDialog = ({ brandId, playerCode, record, onSubmitted, dialog }) => {
    let [submitData, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } = useEditPlayerBank(
        {
            playerCode,
            id: record?.id
        },
        submitData
    )

    const onSubmit = (data = {}) => {
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
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(record)}
                onSubmit={onSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
            >
                <EditBody brandId={brandId} />
            </FormDialog>
        </Backdrop>
    )
}

const EditBody = ({ brandId }) => {
    let {
        isLoading: isLoadingBrandPaymentList,
        data,
        error
    } = useBrandPaymentList({
        brandId,
        transferType: BrandPaymentTransferType.Withdrawal,
        shoAll: true
    })
    let { watch } = useFormContext()

    let BankTypeFieldName = "bankType"
    let bankType = watch(BankTypeFieldName)
    let paymentChannelIdRange =
        data?.items
            ?.filter((item) => item.bankType === bankType)
            ?.map(({ paymentChannelId }) => paymentChannelId) ?? []

    let isBank = PaymentMethodBankType.isBank(bankType)
    let isCrypto = PaymentMethodBankType.isCrypto(bankType)

    return (
        <FieldValues>
            <Box alignY="center">
                <PaymentBankTypeTrans />
            </Box>
            <BrandPaymentMethodBankTypeSelect name={BankTypeFieldName} />

            <Box alignY="center">
                <PaymentChannelTrans />
            </Box>
            <PaymentChannelSelect
                name="paymentChannelId"
                isLoadingRange={isLoadingBrandPaymentList}
                range={paymentChannelIdRange}
                required
            />

            {isBank && (
                <>
                    {/* <RealnameLabel />
                    <Box>
                        <Input name="realName" />
                    </Box> */}

                    <Box alignY="center">
                        <BankLabel />
                    </Box>
                    <Box>
                        <Input name="detail.name" />
                    </Box>

                    <Box alignY="center">
                        <BankAccount />
                    </Box>
                    <Box>
                        <Input name="detail.account" />
                    </Box>
                </>
            )}

            {isCrypto && (
                <>
                    <Trans zh="钱包地址" en="Wallet address" />
                    <Box>
                        <Input name="detail.address" />
                    </Box>
                </>
            )}
        </FieldValues>
    )
}

const DeleteDialog = ({ playerCode, bank, onSubmitted, dialog }) => {
    let [id, setDeleteId] = useState()
    let { result, isLoading, error, deleteCache, mutate } = useDeletePlayerBank(
        playerCode,
        id
    )
    useEffect(() => {
        if (result && !error) {
            onSubmitted?.()
            dialog.hide()
        }
    }, [result, error])

    const reset = () => {
        deleteCache()
        mutate(false, undefined)
        setDeleteId(undefined)
    }

    return (
        <Modal
            showActionButtons
            actionButtonsProps={{
                isLoading,
                onClickSubmit: () => setDeleteId(bank.id),
                onClickCancel: dialog.hide,
                cancelText: <CancelText />,
                submitText: <SubmitText />
            }}
            {...dialog}
        >
            {error && <AlertDialog {...error} onClickClose={reset} />}
            <Box alignX="center" margin="major-1">
                <Trans
                    zh="是否删除此银行卡？"
                    en="Do you want to delete this card?"
                />
            </Box>
        </Modal>
    )
}
