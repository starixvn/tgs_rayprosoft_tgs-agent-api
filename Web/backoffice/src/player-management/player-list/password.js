import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
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
import { Password, Password as PasswordField } from "../../input/Field"
import { FieldValues } from "../../layout/Grid"
import AlertDialog from "../../dialog/Alert"
import { Trans, FormattedDateTime, FormattedNumber } from "../../i18n"
import { useEditPlayerPassword } from "./client"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"

export const Key = "password"

export const Tab = (props = {}) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="修改密码" en="Update Password" />
    </_Tab>
  )
}

export const Panel = ({
  playerCode = "",
  onSubmitted,
  onClickClose,
  ...props
}) => {
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useEditPlayerPassword({
    playerCode,
    data: submitData
  })
  useEffect(() => {
    if (result) {
      onSubmitted && onSubmitted()
    }
  }, [result])

  const submit = (data) => {
    setSubmit(data)
  }

  const clearResult = () => {
    setSubmit(null)
  }

  return (
    <TabPanel tabId={Key} {...props}>
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          password: "",
          confirmPassword: ""
        }}
        onSubmit={submit}
      >
        <EditForm isLoading={isLoading} onClickClose={onClickClose} />
      </Formik>
    </TabPanel>
  )
}

const EditForm = ({ isLoading, onClickClose, ...props }) => {
  let { values = {}, submitForm } = useFormikContext()
  let { password = "" } = values
  return (
    <Form>
      <Stack spacing="major-1" margin="major-1">
        <FieldValues>
          <NewPasswordLabel />
          <PasswordField name="password" />

          <ConfirmPasswordLabel />
          <PasswordField name="confirmPassword" matchPassword={password} />
        </FieldValues>
        <Box alignX="right">
          <SubmitCancel
            isLoading={isLoading}
            onClickSubmit={submitForm}
            onClickCancel={onClickClose}
          />
        </Box>
      </Stack>
    </Form>
  )
}

const NewPasswordLabel = () => <Trans zh="新密码" en="New password" />

const ConfirmPasswordLabel = () => (
  <Trans zh="确认新密码" en="Confirm password" />
)
