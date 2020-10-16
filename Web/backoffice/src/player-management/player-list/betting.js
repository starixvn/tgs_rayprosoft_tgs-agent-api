import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
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
import { Trans } from "../../i18n"

export const Key = "betting"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="投注记录" en="Betting records" />
    </_Tab>
  )
}

export const Panel = ({ playerCode, ...props }) => {
  return (
    <_TabPanel {...props} id={Key}>
      TODO
    </_TabPanel>
  )
}
