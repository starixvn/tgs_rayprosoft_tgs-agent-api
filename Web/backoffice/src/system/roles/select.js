import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
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
import { useRoleList } from "./client"
import {
  useUserBrandId,
  useUserLicenseeId,
  useUserResellerId,
  useUserRoleType
} from "../../user/context"

export const Field = ({ required, validate, ...props }) => {
  let { name } = props
  return (
    <_Field name={name} required={required}>
      {(props) => <Select {...props} />}
    </_Field>
  )
}

export const Select = ({ ...props }) => {
  let userResellerId = useUserResellerId()
  let userLicenseeId = useUserLicenseeId()
  let userBrandId = useUserBrandId()

  let { error, data = {}, isLoading } = useRoleList({
    showAll: true,
    resellerId: userResellerId,
    licenseeId: userLicenseeId,
    brandId: userBrandId
  })
  let { items = [] } = data
  let options = items.map(({ id, name }) => ({
    label: name,
    value: id
  }))

  return (
    <>
      <_Select
        isLoading={isLoading}
        options={[{ label: "", value: "" }, ...options]}
        {...props}
      />
      {error && <AlertDialog {...error} />}
    </>
  )
}
