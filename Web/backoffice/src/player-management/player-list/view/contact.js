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
import { FieldValuePair as FieldValueGrid } from "../../../layout/Grid"

export const Key = "contact"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      {title()}
    </_Tab>
  )
}

export const Panel = ({ player = {}, ...props }) => {
  let { phone, email, qq, weChat } = player
  return (
    <_TabPanel {...props} id={Key}>
      <FieldValueGrid margin="major-1"></FieldValueGrid>
    </_TabPanel>
  )
}

//TODO: i18n
const title = () => "联系信息"
