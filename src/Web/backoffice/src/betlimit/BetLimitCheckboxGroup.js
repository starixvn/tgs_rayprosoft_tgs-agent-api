import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Switch, Box } from "bumbag"
import { useBetLimitList } from "./client"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllPropChecked } from "../checkbox/isAllChecked"
import { Trans } from "../i18n"
import { Input } from "../input/Input"

export const BetLimitCheckboxGroupTable = ({
  name = "betLimits",
  fieldArrayContext,
  width = "100%",
  disabled,
  ...props
}) => {
  let { control, setValue, watch } = useFormContext()

  const { fields } = fieldArrayContext

  let selection = watch(name)
  let allChecked = isAllPropChecked(selection)

  return useMemo(() => {
    return (
      <Set orientation="vertical" width={width} {...props}>
        <Table width={width}>
          <Table.Head>
            <BetLimitCheckboxGroupTableHeadRow
              name={name}
              fields={fields}
              checkedFieldName={defaultCheckedFieldName}
              allChecked={allChecked}
              disabled={disabled}
            />
          </Table.Head>
          <Table.Body>
            {fields.map((betLimitField, index) => (
              <BetLimitCheckboxGroupTableRow
                key={betLimitField._id}
                name={name}
                disabled={disabled}
                field={betLimitField}
                index={index}
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

export const BetLimitCheckboxGroupTableHeadRow = ({
  name,
  fields,
  checkedFieldName = defaultCheckedFieldName,
  allChecked,
  onCheckedChange,
  disabled,
  addonAfter
}) => {
  let { control, setValue, watch, register } = useFormContext()

  return useMemo(() => {
    return (
      <Table.Row>
        <Table.HeadCell>
          <Checkbox
            checked={allChecked}
            onChange={(event) => {
              let checked = event.target.checked
              fields.forEach((field, index) => {
                let fieldName = checkedFieldName(name, field, index)
                setValue(fieldName, checked)
              })

              onCheckedChange?.(checked)
            }}
            disabled={disabled}
          />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="币种" en="Currency" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="最低投注" en="Min Bet" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="最高投注" en="Max Bet" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="单场最高投注" en="Max Bet Per Match" />
        </Table.HeadCell>
        {addonAfter}
      </Table.Row>
    )
  }, [JSON.stringify(fields), allChecked])
}

const defaultCheckedFieldName = (name, field, index) =>
  `${name}[${index}].checked`

export const BetLimitCheckboxGroupTableRow = ({
  name,
  disabled,
  field,
  index,
  checkedFieldName = defaultCheckedFieldName,
  onCheckedChange,
  validateCheckbox,
  checkboxState,
  addonAfter,
  ...props
}) => {
  let formContext = useFormContext()
  return useMemo(() => {
    let checkboxName = defaultCheckedFieldName(name, field, index)
    return (
      <Table.Row>
        <Table.Cell>
          <Box>
            <Input
              type="hidden"
              name={`${name}[${index}].id`}
              value={field.id}
            />
            <Input
              type="hidden"
              name={`${name}[${index}].currencyCode`}
              value={field.currencyCode}
            />
            <Checkbox
              key={field._id}
              name={checkboxName}
              state={checkboxState}
              checkboxRef={formContext.register({ validate: validateCheckbox })}
              defaultChecked={field.checked}
              disabled={disabled}
              onChange={(event) => {
                let checked = event.target.checked
                onCheckedChange?.(checked)
              }}
            />
          </Box>
        </Table.Cell>
        <Table.Cell textAlign="center">{field?.currencyCode}</Table.Cell>
        <Table.Cell textAlign="center">{field?.minBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBetPerMatch}</Table.Cell>
        {addonAfter}
      </Table.Row>
    )
  }, [field, validateCheckbox, checkboxState])
}
