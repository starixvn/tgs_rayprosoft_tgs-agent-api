import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  useTabState,
  Tab as _Tab,
  TabList,
  TabPanel as _TabPanel
} from "reakit/Tab"
import {
  Icon,
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Tabs as _Tabs,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { FieldValues, TwoColumns } from "../../layout/Grid"
import { Field as InputField } from "../../input/Field"
import { Trans, FormattedDateTime, FormattedNumber } from "../../i18n"
import { faTrash, faTrashAlt, faMinus, faEdit, faCog } from "@fortawesome/free-solid-svg-icons"
import { SubmitCancel } from "../../button/ActionButtons"
import { useEditPlayerBank, useDeletePlayerBank } from "./client"
import AlertDialog from "../../dialog/Alert"
import Backdrop from "../../dialog/Backdrop"

export const Key = "banks"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="银行卡信息" en="Bank" />
    </_Tab>
  )
}

export const Panel = ({ playerCode, banks = [], edit = false, onSubmitted, ...props }) => {
  return (
    <_TabPanel {...props} id={Key}>
      <TwoColumns>
        {banks.map((bank, index) => <Bank key={index} 
                                        edit={edit} 
                                        onSubmitted={onSubmitted} 
                                        playerCode={playerCode}
                                        bank={bank} />)}
      </TwoColumns>
      {edit && (
        <Box>
          <Card 
            margin="major-1"
            css={css`
              flex: 1 50%;
            `}>
            TODO: add button
          </Card>
        </Box>
        )}
    </_TabPanel>
  )
}

const Bank = ({ playerCode, bank = {}, edit = false, onSubmitted }) => {
  let { realName = "", bankName = "", bankAccount = "" } = bank
  let deleteDialog = Modal.useState()
  let editDialog = Modal.useState()

  return (
    <Card
      variant="bordered"
      margin="major-1"
      css={css`
        flex: 1 50%;
      `}>
      {edit && (
        <Set alignX="right">
          <Modal.Disclosure use={Button} palette="danger" variant="outlined" {...deleteDialog}>
            <Icon icon={faMinus} fontSize="300"  type="font-awesome" />
          </Modal.Disclosure>
          {deleteDialog.visible && <DeleteDialog playerCode={playerCode} bank={bank} onSubmitted={onSubmitted} dialog={deleteDialog}/>}
          <Modal.Disclosure use={Button} {...editDialog}>
            <Icon icon={faCog} fontSize="300"  type="font-awesome" />
          </Modal.Disclosure>
          {editDialog.visible && <EditDialog playerCode={playerCode} bank={bank} onSubmitted={onSubmitted} dialog={editDialog}/> }
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

const RealnameLabel = () => <Trans zh="真实姓名" en="Real name" />

const BankLabel = () => <Trans zh="银行" en="Bank" />

const BankAccount = () => <Trans zh="银行卡号" en="Bank Account" />

const EditDialog = ({ playerCode, bank, onSubmitted, dialog, finalFocusRef }) => {
  let [submitData, setSubmit] = useState()
  let { result, isLoading, error } = useEditPlayerBank({ playerCode, id: bank?.id, data: submitData })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
      dialog.hide()
    }
  }, [result])

  const submit = data => {
    setSubmit(data)
  }

  const clearResult = () => {
    setSubmit(null)
  }

  let { realName, bankName, bankAccount } = bank
  return (
    <Backdrop {...dialog}>
      <Dialog.Modal 
        hideOnClickOutside={false} 
        unstable_finalFocusRef={finalFocusRef}
        {...dialog}>
        {error && <AlertDialog {...error} onClickClose={clearResult}/>}
        <Formik 
          initialValues={{
            realName: realName ?? "",
            bankName: bankName ?? "",
            bankAccount: bankAccount ?? ""
          }}
          onSubmit={submit}
          >
            {formikProps => (
              <form onReset={formikProps.handleReset} onSubmit={formikProps.handleSubmit} >
                <FieldValues margin="major-1">
                  <RealnameLabel />
                  <InputField name="realName" />

                  <BankLabel />
                  <InputField name="bankName" />

                  <BankAccount />
                  <InputField name="bankAccount" />
                </FieldValues>
                <Box alignX="right" spacing="major-1">
                  <SubmitCancel 
                    isLoading={isLoading}
                    onClickSubmit={formikProps.submitForm}
                    onClickCancel={dialog.hide}
                    />
                </Box>
              </form>
            )}
        </Formik>
      </Dialog.Modal>
    </Backdrop>
  )
}

const DeleteDialog = ({ playerCode, bank, onSubmitted, dialog, finalFocusRef }) => {
  let [id, setDeleteId] = useState()
  let { result, isLoading, error } = useDeletePlayerBank({ playerCode, id })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
      dialog.hide()
    }
  }, [result])

  return (
    <Backdrop {...dialog}>
      <Dialog.Modal 
        hideOnClickOutside={false} 
        unstable_finalFocusRef={finalFocusRef}
        {...dialog}>
        {error && <AlertDialog {...error} onClickClose={clearResult}/>}
        <Box alignX="center" margin="major-1">
          <Trans zh="是否删除此银行卡？" en="Do you want to delete this card?" />
        </Box>
        <Box alignX="right" spacing="major-1">
          <SubmitCancel 
            isLoading={isLoading}
            onClickSubmit={() => {
              setDeleteId(bank.id)
            }}
            onClickCancel={dialog.hide}
            />
        </Box>
      </Dialog.Modal>
    </Backdrop>
  )
}