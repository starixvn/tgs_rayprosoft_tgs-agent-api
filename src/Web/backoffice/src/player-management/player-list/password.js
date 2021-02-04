import React, { useState, useEffect, useMemo, useRef } from "react"
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
import { FieldValues } from "../../layout/Grid"
import AlertDialog from "../../dialog/Alert"
import { Trans, FormattedDateTime, FormattedNumber } from "../../i18n"
import { useEditPlayerPassword } from "./client"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { PasswordInputField } from "../../input/PasswordInput"
import { FormProvider, useForm, useFormContext } from "react-hook-form"

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
  let formMethods = useForm()

  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useEditPlayerPassword(
    playerCode,
    submitData
  )
  useEffect(() => {
    if (result) {
      onSubmitted?.()
    }
  }, [result])

  const onSubmit = (data) => {
    setSubmit(data)
  }

  const reset = () => {
    setSubmit(undefined)
  }

  return (
    <TabPanel tabId={Key} {...props}>
      {error && <AlertDialog {...error} onClickClose={reset} />}
      <FormProvider {...formMethods}>
        <form onSubmit={formMethods.handleSubmit(onSubmit)}>
          <Body isLoading={isLoading} onClickClose={onClickClose} />
        </form>
      </FormProvider>
    </TabPanel>
  )
}

const Body = ({ isLoading, onClickClose, ...props }) => {
  let { watch } = useFormContext()
  let password = watch("password")
  return (
    <Stack spacing="major-1" margin="major-1">
      <FieldValues>
        <NewPasswordLabel />
        <Box>
          <PasswordInputField name="password" />
        </Box>

        <ConfirmPasswordLabel />
        <Box>
          <PasswordInputField name="confirmPassword" matchPassword={password} />
        </Box>
      </FieldValues>
      <Box alignX="right">
        <SubmitCancel isLoading={isLoading} />
      </Box>
    </Stack>
  )
}

const NewPasswordLabel = () => <Trans zh="新密码" en="New password" />

const ConfirmPasswordLabel = () => (
  <Trans zh="确认新密码" en="Confirm password" />
)
