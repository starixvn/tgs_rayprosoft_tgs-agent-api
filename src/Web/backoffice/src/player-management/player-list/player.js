import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { useEditPlayerInfo } from "./client"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import { PlayerStatusSelect, PlayerStatusLabel } from "../PlayerStatus"
import { VipLevelSelect } from "../../vip-levels/VipLevelSelect"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { PlayerAgentSelect } from "../PlayerAgent"
import { FormProvider, useForm } from "react-hook-form"
import Input from "../../input/Input"

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
  let {
    realName = "",
    vipId = "",
    parentUsername = "",
    status = "",
    hasAgent
  } = player

  let formMethods = useForm({
    defaultValues: {
      realName: realName ?? "",
      vipId,
      parentUsername: parentUsername ?? "",
      status,
      hasAgent: hasAgent.toString()
    }
  })

  let [submitData, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useEditPlayerInfo(
    playerCode,
    submitData
  )

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      onSubmitted?.()
    }
  }, [result])

  const onSubmit = (data = {}) => {
    let hasAgent = data.hasAgent === Boolean(true).toString() ? true : false
    setSubmit({
      ...data,
      hasAgent
    })
  }

  return (
    <TabPanel tabId={Key} {...props}>
      {error && <AlertDialog {...error} onClickClose={reset} />}
      <FormProvider {...formMethods}>
        <form onSubmit={formMethods.handleSubmit(onSubmit)}>
          <EditForm
            player={player}
            isLoading={isLoading}
            onClickClose={reset}
          />
        </form>
      </FormProvider>
    </TabPanel>
  )
}

const EditForm = ({ player = {}, isLoading, onClickClose, ...props }) => {
  let { username = "", registeredOn, balance } = player

  return (
    <Stack spacing="major-1" margin="major-1">
      <FieldValues>
        <UserNameLabel />
        <Text>{username}</Text>

        <RealNameLabel />
        <Box>
          <Input name="realName" required />
        </Box>

        <VIPLevelLabel />
        <Box>
          <VipLevelSelect name="vipId" required placeholder={false} />
        </Box>

        <ParentUsernameLabel />
        <Box>
          <Input name="parentUsername" />
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
          <PlayerStatusSelect name="status" required placeholder={false} />
        </Box>

        <AgentLabel />
        <Box>
          <PlayerAgentSelect name="hasAgent" required />
        </Box>
      </FieldValues>
      <Box alignX="right">
        <SubmitCancel onClickCancel={onClickClose} />
      </Box>
    </Stack>
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
          <PlayerStatusLabel value={status} />
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
