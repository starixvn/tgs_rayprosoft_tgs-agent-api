import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import useSWR from "swr"
import { useEditPlayerInfo } from "./client"
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
  Select as _Select,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import { Field as InputField } from "../../input/Field"
import {
  Field as StatusField,
  from as toStatus,
  toString as toStatusString
} from "../status"
import { Field as VipLevelField } from "../../vip-level/field"
import { Field as _Field, FieldMinHeight } from "../../select/Field"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"

export const Key = "player"

export const Tab = ({ ...props }) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="玩家信息" en="Player" />
    </_Tab>
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
  let { result, error, mutate, isLoading } = useEditPlayerInfo(
    playerCode,
    submitData
  )
  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
    }
  }, [result])

  const submit = (data = {}) => {
    let hasAgent = data.hasAgent === Boolean(true).toString() ? true : false
    let status = toStatus(data.status)
    setSubmit({
      ...data,
      hasAgent,
      status
    })
  }

  const clearResult = () => {
    setSubmit(null)
  }

  let {
    realName = "",
    vipId = "",
    parentUsername = "",
    status,
    hasAgent
  } = player

  return (
    <TabPanel tabId={Key} {...props}>
      {error && <AlertDialog {...error} onClickClose={clearResult} />}
      <Formik
        initialValues={{
          realName: realName ?? "",
          vipId,
          parentUsername: parentUsername ?? "",
          status: toStatusString(status),
          hasAgent: hasAgent.toString()
        }}
        onSubmit={submit}
      >
        <EditForm
          player={player}
          isLoading={isLoading}
          onClickClose={onClickClose}
        />
      </Formik>
    </TabPanel>
  )
}

const EditForm = ({ player = {}, isLoading, onClickClose, ...props }) => {
  let { submitForm } = useFormikContext()
  let { username = "", registeredOn, balance } = player

  let turnOnLabel = useTurnOnLabel()
  let turnOffLabel = useTurnOffLabel()
  return (
    <Form>
      <Stack spacing="major-1" margin="major-1">
        <FieldValues>
          <UserNameLabel />
          <Text>{username}</Text>

          <RealNameLabel />
          <Box>
            <InputField name="realName" required />
          </Box>

          <VIPLevelLabel />
          <Box>
            <VipLevelField name="vipId" required />
          </Box>

          <ParentUsernameLabel />
          <Box>
            <InputField name="parentUsername" />
          </Box>

          <RegistrationLabel />
          <Box>
            <FormattedDateTime value={registeredOn} />
          </Box>

          <BalanceLabel />
          <Box>
            <FormattedNumber value={balance} />
          </Box>

          <StatusLabel />
          <Box>
            <StatusField name="status" required />
          </Box>

          <AgentLabel />
          <Box>
            <_Field name="hasAgent" required>
              {(props) => (
                <_Select
                  options={[
                    { label: turnOnLabel, value: "true" },
                    { label: turnOffLabel, value: "false" }
                  ]}
                  {...props}
                />
              )}
            </_Field>
          </Box>
        </FieldValues>
        <Box alignX="right">
          <SubmitCancel
            onClickSubmit={submitForm}
            onClickCancel={onClickClose}
          />
        </Box>
      </Stack>
    </Form>
  )
}

export const Panel = ({ player = {}, onSubmitted, ...props }) => {
  let {
    username = "",
    realName = "",
    vipLevel = "",
    parentUsername = "",
    registeredOn,
    balance,
    status,
    hasAgent
  } = player

  let turnOnLabel = useTurnOnLabel()
  let turnOffLabel = useTurnOffLabel()
  return (
    <TabPanel tabId={Key} {...props}>
      <FieldValues spacing="major-2">
        <UserNameLabel />
        <Text>{username}</Text>

        <RealNameLabel />
        <Box>
          <Text>{realName}</Text>
        </Box>

        <VIPLevelLabel />
        <Box>
          <Text>{vipLevel}</Text>
        </Box>

        <ParentUsernameLabel />
        <Box>
          <Text>{parentUsername}</Text>
        </Box>

        <RegistrationLabel />
        <Box>
          <FormattedDateTime value={registeredOn} />
        </Box>

        <BalanceLabel />
        <Box>
          <FormattedNumber value={balance} />
        </Box>

        <StatusLabel />
        <Box>
          <StatusLabel value={status} />
        </Box>

        <AgentLabel />
        <Box>
          <Text>
            {hasAgent && turnOnLabel}
            {!hasAgent && turnOffLabel}
          </Text>
        </Box>
      </FieldValues>
    </TabPanel>
  )
}

const UserNameLabel = () => <Trans zh="用户名" en="User name" />

const RealNameLabel = () => <Trans zh="真实姓名" en="Real name" />

const VIPLevelLabel = () => <Trans zh="VIP等级" en="VIP Level" />

const ParentUsernameLabel = () => <Trans zh="上级用户名" en="上级用户名" />

const RegistrationLabel = () => <Trans zh="注册时间" en="Registration" />

const BalanceLabel = () => <Trans zh="余额" en="Balance" />

const StatusLabel = () => <Trans zh="状态" en="Status" />

const AgentLabel = () => <Trans zh="代理功能" en="Agent" />

const useTurnOnLabel = () =>
  useTrans({
    zh: "开启",
    en: "Turn on"
  })

const useTurnOffLabel = () =>
  useTrans({
    zh: "关闭",
    en: "Turn off"
  })
