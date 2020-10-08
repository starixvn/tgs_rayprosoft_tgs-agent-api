import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../../client"
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
  SelectField,
  Tabs,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"

export const Key = "bank-cards"

export const Tab = (props = {}) => (
  <Tabs.Tab tabId={Key} {...props}>
    {title()}
  </Tabs.Tab>
)

export const Panel = (props = {}) => {
  return (
    <Tabs.Panel tabId={Key} {...props}>
      TODO: async player bank cards
    </Tabs.Panel>
  )
}

//TODO: i18n
const title = () => "银行卡信息"
