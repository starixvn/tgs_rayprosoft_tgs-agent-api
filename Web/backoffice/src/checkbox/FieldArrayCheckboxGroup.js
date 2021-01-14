import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Box } from "bumbag"
import {
  useForm,
  useFormContext,
  Controller,
  useFieldArray
} from "react-hook-form"
import Checkbox from "../checkbox/Checkbox"
import { isAllChecked } from "./isAllChecked"
import { useTrans } from "../i18n"

export const FieldArrayCheckboxGroup = ({
  name,
  fieldArrayName,
  showSelectAll = true,
  fieldLabel = defaultFieldLabel,
  width = "100%",
  disabled,
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()
  const { fields } = useFieldArray({
    control,
    name: fieldArrayName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllChecked(selection)
  let selectAll = useTrans({
    zh: "全选",
    en: "Select All"
  })

  return (
    <Set orientation="vertical" width={width} {...props}>
      <Checkbox
        label={selectAll}
        checked={allChecked}
        onChange={(event) => {
          let checked = event.target.checked
          fields.forEach((field, index) => {
            setValue(fieldName(name, field), checked)
          })
        }}
        disabled={disabled}
      />

      <Set>
        {fields.map((field) => (
          <Checkbox
            key={field._id}
            name={fieldName(name, field)}
            checkboxRef={register?.()}
            defaultChecked={field.checked}
            label={fieldLabel(field)}
            disabled={disabled}
          />
        ))}
      </Set>
    </Set>
  )
}

const fieldName = (name, checkboxField) => `${name}.${checkboxField.id}`

const defaultFieldLabel = (field = {}) => field.name ?? ""
