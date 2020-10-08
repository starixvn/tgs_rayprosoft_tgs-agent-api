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
import { FieldValues, TwoColumns } from "../../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber } from "../../../i18n"

export const Key = "banks"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="银行卡信息" en="Bank" />
    </_Tab>
  )
}

//TODO: use grid layout?
export const Panel = ({ banks = [], ...props }) => {
  return (
    <_TabPanel {...props} id={Key}>
      <TwoColumns>
        {banks.length > 0 &&
          banks.map((bank, index) => <Bank key={index} {...bank} />)}
      </TwoColumns>
    </_TabPanel>
  )
}

const Bank = ({ realName = "", bankName = "", bankAccount = "" }) => {
  return (
    <Card
      variant="bordered"
      margin="major-1"
      css={css`
        flex: 1 50%;
      `}
    >
      <FieldValues>
        <Trans zh="真实姓名" en="Real name" />
        <Text>{realName}</Text>
        <Trans zh="银行" en="Bank" />
        <Text>{bankName}</Text>
        <Trans zh="银行卡号" en="Bank Account" />
        <Text>{bankAccount}</Text>
      </FieldValues>
    </Card>
  )
}
