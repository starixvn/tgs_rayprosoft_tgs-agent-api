import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { Formik, Form, useFormikContext } from "formik"
import { get } from "../../client"
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
  InputField,
  Checkbox,
  FieldStack,
  FieldWrapper,
  Select as _Select,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { Field as _Field } from "../../select/Field"
import AlertDialog from "../../dialog/Alert"

export const Field = ({ required, validate, ...props }) => {
  let { name } = props
  return (
    <_Field name={name} required={required}>
      {(props) => <Select {...props} />}
    </_Field>
  )
}

export const Select = ({ visible = true, ...props }) => {
  const deferFn = () => get("/api/department").then(({ items }) => items)

  return (
    <Async deferFn={deferFn}>
      <Async.Initial>
        {({ run: fetchDepartments }) => {
          useEffect(() => {
            fetchDepartments()
          }, [visible])
          return null
        }}
      </Async.Initial>
      <Async.Pending>
        <_Select isLoading options={[]} {...props} />
      </Async.Pending>
      <Async.Resolved>
        {(items) => {
          let options = items.map(({ id, name }) => ({
            label: name,
            value: id
          }))

          return (
            <_Select
              options={[{ label: "", value: "" }, ...options]}
              {...props}
            />
          )
        }}
      </Async.Resolved>
      <Async.Rejected>{(error) => <AlertDialog {...error} />}</Async.Rejected>
    </Async>
  )
}
