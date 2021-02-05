import React, { useState, useEffect, useMemo, useRef, Fragment } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Box } from "bumbag"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllChecked, isAllPropChecked } from "../../checkbox/isAllChecked"
import { Trans, useTrans } from "../../i18n"
import Checkbox from "../../checkbox"
import Input from "../../input/Input"

export const GameProviderCheckboxGroup = ({
  name = "gameProviders",
  gameProviderIdFieldName = "gameProviderId",
  fieldArrayContext,
  disabled,
  width = "100%",
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()

  const { fields } = fieldArrayContext

  let selection = watch(name)
  let allChecked = isAllPropChecked(selection)

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
            setValue(checkedFieldName(name, field, index), checked)
          })
        }}
        disabled={disabled}
      />

      <Set>
        {fields.map((field, index) => (
          <Box key={field._id}>
            <Input
              type="hidden"
              name={`${name}[${index}].${gameProviderIdFieldName}`}
              value={field[gameProviderIdFieldName]}
            />
            <Checkbox
              key={field._id}
              name={checkedFieldName(name, field, index)}
              checkboxRef={register?.()}
              defaultChecked={field.checked}
              label={field.name}
              disabled={disabled}
            />
          </Box>
        ))}
      </Set>
    </Set>
  )
}

export const toCheckedGameProviderIds = (
  fieldName = "gameProviderId",
  gameProviders = []
) => {
  return gameProviders
    .filter(({ checked }) => checked)
    .map((gameProvider) => {
      return Reflect.get(gameProvider, fieldName)
    })
}

const checkedFieldName = (name, field, index) => `${name}[${index}].checked`
