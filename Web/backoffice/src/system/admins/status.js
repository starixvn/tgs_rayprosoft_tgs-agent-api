import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import client from "../../client"
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
  Select as _Select,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { Field as _Field } from "../../select/Field"

const Disabled = 0
const Enabled = 1

export const from = (value) => {
  if (value === "Disabled") {
    return Disabled
  }
  if (value === Enabled) {
    return Enabled
  }
  return value
}

export const Select = ({ name, required, ...props }) => {
  return (
    <_Field name={name} required={required}>
      {(props) => <_Select options={statusOptions()} {...props} />}
    </_Field>
  )
}

const statusOptions = () => [
  {
    label: "",
    value: null
  },
  {
    label: label(Disabled),
    value: Disabled
  },
  {
    label: label(Enabled),
    value: Enabled
  }
]

//TODO: i18n
export const label = (value) => {
  if (value === "Enabled" || value === Disabled) {
    return "活跃"
  }

  if (value === "Disabled" || value === Enabled) {
    return "封锁"
  }

  return value
}
