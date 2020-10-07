import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../../client"
import useSWR from "swr"
import { usePlayer } from "../player"
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
import { FieldValuePair as FieldValueGrid } from "../../../layout/Grid"

export const Key = "player"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      {title()}
    </_Tab>
  )
}

export const Panel = ({ player, ...props }) => {
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
  return (
    <_TabPanel {...props} id={Key}>
      <FieldValueGrid margin="major-1">
        <Text>{usernameField()}</Text>
        <Text>{username}</Text>
        <Text>{realNameField()}</Text>
        <Text>{realName}</Text>
        <Text>{vipLevelField()}</Text>
        <Text>{vipLevel}</Text>
        <Text>{parentUsernameField()}</Text>
        <Text>{parentUsername}</Text>
        <Text>{registeredOnField()}</Text>
        <Text>TODO: fmt {registeredOn}</Text>
        <Text>{balanceField()}</Text>
        <Text>TODO: fmt {balance}</Text>
        <Text>{statusField()}</Text>
        <Text>TODO: trans {status}</Text>
        <Text>{agentField()}</Text>
        <Text>TODO: {hasAgent}</Text>
      </FieldValueGrid>
    </_TabPanel>
  )
}

//TODO: i18n
const title = () => "玩家信息"

const usernameField = () => "用户名"

const realNameField = () => "真实姓名"

const vipLevelField = () => "VIP等级"

const parentUsernameField = () => "上级用户名"

const registeredOnField = () => "注册时间"

const balanceField = () => "余额"

const statusField = () => "状态"

const agentField = () => "代理功能"
