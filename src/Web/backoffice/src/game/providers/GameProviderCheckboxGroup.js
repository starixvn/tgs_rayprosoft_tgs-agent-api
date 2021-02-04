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
          <Fragment key={index}>
            <Box>
              <Input
                type="hidden"
                name={`${name}[${index}].id`}
                value={field.id}
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
          </Fragment>
        ))}
      </Set>
    </Set>
  )
}

export const toCheckedGameProviderIds = (gameProviders = []) => {
  return gameProviders.filter(({ checked }) => checked).map(({ id }) => id)
}

const checkedFieldName = (name, field, index) => `${name}[${index}].checked`
