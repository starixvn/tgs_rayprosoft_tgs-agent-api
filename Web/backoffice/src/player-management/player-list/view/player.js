import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../../client"
import useSWR from "swr"
import { usePlayer } from "../client"
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
import { FieldValues } from "../../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber } from "../../../i18n"
import { Label as StatusLabel } from "../status"

export const Key = "player"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="玩家信息" en="Player" />
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
      <FieldValues margin="major-1">
        <Trans zh="用户名" en="User name" />
        <Text>{username}</Text>

        <Trans zh="真实姓名" en="Real name" />
        <Text>{realName}</Text>

        <Trans zh="VIP等级" en="VIP Level" />
        <Text>{vipLevel}</Text>

        <Trans zh="上级用户名" en="上级用户名" />
        <Text>{parentUsername}</Text>

        <Trans zh="注册时间" en="Registration" />
        <FormattedDateTime value={registeredOn} />

        <Trans zh="余额" en="Balance" />
        <FormattedNumber value={balance} />

        <Trans zh="状态" en="Status" />
        <StatusLabel value={status} />

        <Trans zh="代理功能" en="Agent" />
        <Text>
          {hasAgent && <Trans zh="开启" en="Turn on" />}
          {!hasAgent && <Trans zh="关闭" en="Turn off" />}
        </Text>
        <Text>{hasAgent}</Text>
      </FieldValues>
    </_TabPanel>
  )
}
