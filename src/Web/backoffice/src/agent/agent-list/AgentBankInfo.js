import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Set,
    Button,
    Text,
    Table,
    Icon
} from "bumbag"
import { faMinus, faCog, faPlus } from "@fortawesome/free-solid-svg-icons"
import { FieldValues, TwoColumns } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { Modal, useModalState } from "../../dialog/Modal"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import {
    useAddAgentBank,
    useDeleteAgentBank,
    useUpdateAgentBank
} from "./client"
import { CancelText, SubmitText } from "../../button/ActionButtons"
import Backdrop from "../../dialog/Backdrop"
import FormDialog from "../../form/FormDialog"
import Disclosure from "../../dialog/Disclosure"
import PaymentMethodBankType from "../../payment/method/PaymentMethodBankType"

const Key = "AgentBankInfo"

export const AgentBankInfoTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="银行卡信息" en="Agent Bank Info" />
        </_Tab>
    )
}

export const AgentBankInfoPanel = ({ readonly = false, ...props }) =>
    readonly ? <ViewPanel {...props} /> : <EditPanel {...props} />

const EditPanel = ({ record, onSubmitted, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <TwoColumns>
                {record?.banks?.map?.((bank, index) => (
                    <Bank key={index} record={bank} onSubmitted={onSubmitted} />
                ))}
            </TwoColumns>
            <AddBank record={record} onSubmitted={onSubmitted} />
        </TabPanel>
    )
}

const ViewPanel = ({ record = {}, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <TwoColumns>
                {record?.banks?.map?.((bank, index) => (
                    <Bank key={index} record={bank} readOnly />
                ))}
            </TwoColumns>
        </TabPanel>
    )
}

const Bank = ({ record = {}, readOnly = false, onSubmitted }) => {
    let { bankType = "" } = record
    let deleteDialog = useModalState()
    let editDialog = useModalState()

    let editable = !readOnly
    let isBank = PaymentMethodBankType.isBank(bankType)
    let isCrypto = PaymentMethodBankType.isCrypto(bankType)
    return (
        <Card>
            {editable && (
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
                            bank={record}
                            onSubmitted={onSubmitted}
                            dialog={editDialog}
                        />
                    )}
                </Set>
            )}
            <FieldValues>
                <Trans zh="真实姓名" en="Real name" />
                <Text>{record?.realName ?? ""}</Text>

                <Trans zh="银行" en="Bank" />
                <Text>{record?.bankName ?? ""}</Text>

                <Trans zh="银行卡号" en="Bank Account" />
                <Text>{record?.bankAccount ?? ""}</Text>
            </FieldValues>
        </Card>
    )
}

const AddBank = ({ record, onSubmitted }) => {
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
                    agentId={record?.id}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddDialog = ({ agentId, onSubmitted, dialog }) => {
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } = useAddAgentBank(
        agentId,
        formData
    )

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={{
                    realName: "",
                    bankName: "",
                    bankAccount: ""
                }}
                onSubmit={setSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
            >
                <EditBody />
            </FormDialog>
        </Backdrop>
    )
}

const EditDialog = ({ bank, onSubmitted, dialog }) => {
    let id = bank?.id
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } = useUpdateAgentBank(
        id,
        formData
    )

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={{ ...bank }}
                onSubmit={setSubmit}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
            >
                <EditBody />
            </FormDialog>
        </Backdrop>
    )
}

const EditBody = ({}) => {
    return (
        <FieldValues>
            <Trans zh="真实姓名" en="Real name" />
            <Box>
                <Input name="realName" />
            </Box>

            <Trans zh="银行" en="Bank" />
            <Box>
                <Input name="bankName" />
            </Box>

            <Trans zh="银行卡号" en="Bank Account" />
            <Box>
                <Input name="bankAccount" />
            </Box>
        </FieldValues>
    )
}

const DeleteDialog = ({ bank, onSubmitted, dialog }) => {
    let [id, setDeleteId] = useState()
    let { result, isLoading, error, deleteCache, mutate } =
        useDeleteAgentBank(id)

    useEffect(() => {
        if (result && !error) {
            onSubmitted?.()
            dialog.hide()
        }
    }, [result, error])

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
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
