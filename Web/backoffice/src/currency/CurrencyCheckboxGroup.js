import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text } from "bumbag"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { Trans } from "../i18n"
import { isAllChecked } from "../checkbox/isAllChecked"
import { FieldArrayCheckboxGroup } from "../checkbox/FieldArrayCheckboxGroup"

export const CurrencyCheckboxGroupTable = ({
  name = "currencyCodes",
  fieldArrayName = "currencies",
  width = "100%",
  disabled = false,
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()
  let currencies = watch(fieldArrayName)
  const { fields } = useFieldArray({
    control,
    name: fieldArrayName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllChecked(selection)

  return (
    <Set orientation="vertical" width={width}>
      <Table width={width}>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>
              <Checkbox
                checked={allChecked}
                onChange={(event) => {
                  let checked = event.target.checked
                  fields.forEach((field, index) => {
                    setValue(fieldName(name, field), checked)
                  })
                }}
                disabled={disabled}
              />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="代码" en="Code" />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="币种" en="Currency" />
            </Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {fields.map((field) => (
            <Row
              key={field._id}
              name={name}
              disabled={disabled}
              field={field}
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const Row = ({ name, disabled, field }) => {
  let formContext = useFormContext()
  return (
    <Table.Row>
      <Table.Cell>
        <Checkbox
          key={field._id}
          name={fieldName(name, field)}
          checkboxRef={formContext?.register?.()}
          defaultChecked={field.checked}
          disabled={disabled}
        />
      </Table.Cell>
      <Table.Cell>{field.code}</Table.Cell>
      <Table.Cell>{field.name}</Table.Cell>
    </Table.Row>
  )
}

const fieldName = (name, checkboxField) => `${name}.${checkboxField.code}`
