import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useEditPlayerContact } from "./client"
import { FormProvider, useForm } from "react-hook-form"
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
  useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import HookFormInputField from "../../input/Input"
import EmailInput from "../../input/EmailInput"
import PhoneInput from "../../input/PhoneInput"

export const Key = "contact"

export const Tab = ({ ...props }) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="联系信息" en="Contact Info" />
    </_Tab>
  )
}

export const Panel = ({ player = {}, ...props }) => {
  let { phone, email, qq, weChat } = player
  return (
    <TabPanel tabId={Key} {...props}>
      <FieldValues spacing="major-2">
        <MobileLabel />
        <Text>{phone}</Text>

        <EmailLabel />
        <Text>{email}</Text>

        <QQLabel />
        <Text>{qq}</Text>

        <WeChatLabel />
        <Text>{weChat}</Text>
      </FieldValues>
    </TabPanel>
  )
}

export const EditPanel = ({
  playerCode = "",
  player = {},
  onSubmitted,
  onClickClose,
  ...props
}) => {
  let { phone, email, qq, weChat } = player
  let formMethods = useForm({
    defaultValues: {
      phone: phone ?? "",
      email: email ?? "",
      qq: qq ?? "",
      weChat: weChat ?? ""
    }
  })

  let [submitData, setSubmit] = useState()
  let { result, error, mutate } = useEditPlayerContact(playerCode, submitData)
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
    }
  }, [result])

  const onSubmit = setSubmit

  const clearResult = () => setSubmit(null)

  return (
    <TabPanel tabId={Key} {...props}>
      <FormProvider {...formMethods}>
        <form onSubmit={formMethods.handleSubmit(onSubmit)}>
          <Body onClickClose={onClickClose} />
        </form>
      </FormProvider>
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
    </TabPanel>
  )
}

const Body = ({ onClickClose }) => {
  return (
    <Stack spacing="major-1" margin="major-1">
      <FieldValues>
        <MobileLabel />
        <Box>
          <PhoneInput name="phone" />
        </Box>

        <EmailLabel />
        <Box>
          <EmailInput name="email" />
        </Box>

        <QQLabel />
        <Box>
          <HookFormInputField name="qq" />
        </Box>

        <WeChatLabel />
        <Box>
          <HookFormInputField name="weChat" />
        </Box>
      </FieldValues>
      <Box alignX="right">
        <SubmitCancel onClickCancel={onClickClose} />
      </Box>
    </Stack>
  )
}

const MobileLabel = () => <Trans zh="手机号码" en="Mobile" />

const EmailLabel = () => <Trans zh="邮箱" en="Email" />

const QQLabel = () => <Trans zh="QQ" en="QQ" />

const WeChatLabel = () => <Trans zh="微信号" en="WeChat" />
