import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../../client"
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
import { Trans } from "../../../i18n"

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
        <Trans zh="手机号码" en="Mobile" />
        <Text>{phone}</Text>
        <Trans zh="邮箱" en="Email" />
        <Text>{email}</Text>
        <Trans zh="QQ" en="QQ" />
        <Text>{qq}</Text>
        <Trans zh="微信号" en="WeChat" />
        <Text>{weChat}</Text>
      </FieldValues>
    </_TabPanel>
  )
}
