import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useEditPlayerContact } from "./client"
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
  useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, useTrans } from "../../i18n"
import {
  Field as InputField,
  Email as EmailField,
  Phone as PhoneField
} from "../../input/Field"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"

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
  let [submitData, setSubmit] = useState()
  let { result, error, mutate } = useEditPlayerContact(playerCode, submitData)
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
    }
  }, [result])

  const submit = setSubmit

  const clearResult = () => setSubmit(null)
  let { phone, email, qq, weChat } = player
  return (
    <TabPanel tabId={Key} {...props}>
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          phone: phone ?? "",
          email: email ?? "",
          qq: qq ?? "",
          weChat: weChat ?? ""
        }}
        onSubmit={submit}
      >
        <Form>
          <FormBody onClickClose={onClickClose} />
        </Form>
      </Formik>
    </TabPanel>
  )
}

const FormBody = ({ onClickClose }) => {
  let { submitForm } = useFormikContext()
  return (
    <Stack spacing="major-1" margin="major-1">
      <FieldValues>
        <MobileLabel />
        <PhoneField name="phone" />

        <EmailLabel />
        <EmailField name="email" />

        <QQLabel />
        <InputField name="qq" />

        <WeChatLabel />
        <InputField name="weChat" />
      </FieldValues>
      <Box alignX="right">
        <SubmitCancel onClickSubmit={submitForm} onClickCancel={onClickClose} />
      </Box>
    </Stack>
  )
}

const MobileLabel = () => <Trans zh="手机号码" en="Mobile" />

const EmailLabel = () => <Trans zh="邮箱" en="Email" />

const QQLabel = () => <Trans zh="QQ" en="QQ" />

const WeChatLabel = () => <Trans zh="微信号" en="WeChat" />
