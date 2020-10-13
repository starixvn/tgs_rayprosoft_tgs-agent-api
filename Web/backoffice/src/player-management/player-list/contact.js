import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { useEditPlayerContact } from "./client"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  useTabState,
  Tab as _Tab,
  TabList,
  TabPanel as _TabPanel
} from "reakit/Tab"
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
import { Trans, useTrans } from "../../i18n"
import { Field as InputField, Email as EmailField, Phone as PhoneField } from "../../input/Field"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"

export const Key = "contact"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="联系信息" en="Contact Info" />
    </_Tab>
  )
}

export const Panel = ({ player = {}, ...props }) => {
  let { phone, email, qq, weChat } = player
  return (
    <_TabPanel {...props} id={Key}>
      <FieldValues margin="major-1">
        <MobileLabel />
        <Text>{phone}</Text>
        
        <EmailLabel />
        <Text>{email}</Text>
        
        <QQLabel />
        <Text>{qq}</Text>
        
        <WeChatLabel />
        <Text>{weChat}</Text>
        
      </FieldValues>
    </_TabPanel>
  )
}

export const EditPanel = ({ playerCode = "", player = {}, onSubmitted, onClickClose, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate } = useEditPlayerContact({ playerCode, data: submitData })
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
    }
  }, [result])

  const submit = setSubmit

  const clearResult =  () => setSubmit(null)
  let { phone, email, qq, weChat } = player
  return (
    <_TabPanel {...props} id={Key}>
      {error && <AlertDialog {...error} onClickClose={clearResult}/>}
      <Formik 
        initialValues={{
          phone,
          email,
          qq,
          weChat
        }}
        onSubmit={submit}>
        <Form>
          <FormBody onClickClose={onClickClose} />
        </Form>
      </Formik>

    </_TabPanel>
  )
}

const FormBody = ({ onClickClose }) => {
  let { submitForm } = useFormikContext()
  //TODO: any required fields?  
  return (      
    <>
      <FieldValues margin="major-1">
        <MobileLabel />
        <PhoneField name="phone"/>
        
        <EmailLabel />
        <EmailField name="email" />
        
        <QQLabel />
        <InputField name="qq" />
        
        <WeChatLabel />
        <InputField name="weChat" />

      </FieldValues>
      <Box alignX="right">
        <SubmitCancel 
          onClickSubmit={submitForm}
          onClickCancel={onClickClose}
          /> 
      </Box> 
    </>
  )
}

const MobileLabel = () => <Trans zh="手机号码" en="Mobile" />

const EmailLabel = () => <Trans zh="邮箱" en="Email" />

const QQLabel = () => <Trans zh="QQ" en="QQ" />

const WeChatLabel = () => <Trans zh="微信号" en="WeChat" />