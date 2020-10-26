import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import useSWR from "swr"
import { useEditPlayerInfo } from "./client"
import { Formik, Field, Form, useFormikContext } from "formik"
/*
import {
  useTabState,
  Tab as _Tab,
  TabList,
  TabPanel as _TabPanel
} from "reakit/Tab"
*/
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
  Tabs,
  Dialog,
  Modal,
  Table,
  useClassName,
  ActionButtons
} from "bumbag"

export const Panel = ({ children, ...props }) => {
  //TODO: active prop?
  return (
    <Tabs.Panel {...props}>
      <Stack margin="major-2" spacing="major-2">
        {children}
      </Stack>
    </Tabs.Panel>
  )
}
