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
  useClassName
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

export const Key = "banks"

export const Tab = ({ ...props }) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="银行卡信息" en="Bank" />
    </_Tab>
  )
}

export const Panel = ({
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
            playerCode={playerCode}
            bank={bank}
            onSubmitted={onSubmitted}
            edit={edit}
          />
        ))}
      </TwoColumns>
      {edit && <AddBank playerCode={playerCode} onSubmitted={onSubmitted} />}
    </TabPanel>
  )
}

const Bank = ({ playerCode, bank = {}, edit = false, onSubmitted }) => {
  let { realName = "", bankName = "", bankAccount = "" } = bank
  let deleteDialog = useModalState()
  let editDialog = useModalState()

  return (
    <Card>
      {edit && (
        <Set alignX="right">
          <Disclosure palette="danger" variant="outlined" {...deleteDialog}>
            <Icon icon={faMinus} fontSize="300" type="font-awesome" />
          </Disclosure>
          {deleteDialog.visible && (
            <DeleteDialog
              playerCode={playerCode}
              bank={bank}
              onSubmitted={onSubmitted}
              dialog={deleteDialog}
            />
          )}
          <Disclosure {...editDialog}>
            <Icon icon={faCog} fontSize="300" type="font-awesome" />
          </Disclosure>
          {editDialog.visible && (
            <EditDialog
              playerCode={playerCode}
              bank={bank}
              onSubmitted={onSubmitted}
              dialog={editDialog}
            />
          )}
        </Set>
      )}
      <FieldValues>
        <RealnameLabel />
        <Text>{realName}</Text>

        <BankLabel />
        <Text>{bankName}</Text>

        <BankAccount />
        <Text>{bankAccount}</Text>
      </FieldValues>
    </Card>
  )
}

const AddBank = ({ playerCode, onSubmitted }) => {
  let dialog = useModalState()
  return (
    <>
      <TwoColumns>
        <Card alignX="center">
          <Disclosure {...dialog}>
            <Icon icon={faPlus} fontSize="300" type="font-awesome" />
          </Disclosure>
        </Card>
      </TwoColumns>
      {dialog.visible && (
        <AddDialog
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

const RealnameLabel = () => <Trans zh="真实姓名" en="Real name" />

const BankLabel = () => <Trans zh="银行" en="Bank" />

const BankAccount = () => <Trans zh="银行卡号" en="Bank Account" />

const AddDialog = ({ playerCode, onSubmitted, dialog, finalFocusRef }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useAddPlayerBank(playerCode, submitData)

  const reset = () => setSubmit(undefined)

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

const EditDialog = ({ playerCode, bank, onSubmitted, dialog }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useEditPlayerBank(
    {
      playerCode,
      id: bank?.id
    },
    submitData
  )

  const clearResult = () => setSubmit(undefined)

  return (
    <Backdrop {...dialog}>
      <FormDialog
        defaultValues={{ ...bank }}
        onSubmit={setSubmit}
        isLoading={isLoading}
        result={result}
        error={error}
        onCloseErrorAlert={clearResult}
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
      <RealnameLabel />
      <Box>
        <Input name="realName" />
      </Box>

      <BankLabel />
      <Box>
        <Input name="bankName" />
      </Box>

      <BankAccount />
      <Box>
        <Input name="bankAccount" />
      </Box>
    </FieldValues>
  )
}

const DeleteDialog = ({ playerCode, bank, onSubmitted, dialog }) => {
  let [id, setDeleteId] = useState()
  let { result, isLoading, error } = useDeletePlayerBank(playerCode, id)
  useEffect(() => {
    if (result && !error) {
      onSubmitted?.()
      dialog.hide()
    }
  }, [result, error])

  const clearResult = () => setDeleteId(undefined)

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
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Box alignX="center" margin="major-1">
        <Trans zh="是否删除此银行卡？" en="Do you want to delete this card?" />
      </Box>
    </Modal>
  )
}
