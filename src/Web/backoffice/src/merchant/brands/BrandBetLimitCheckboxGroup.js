import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, Icon, Switch, Checkbox, Box } from "bumbag"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import { Trans, useTrans } from "../../i18n"
import Input from "../../input/Input"
import {
  BetLimitCheckboxGroupTableHeadRow,
  BetLimitCheckboxGroupTableRow
} from "../../betlimit/BetLimitCheckboxGroup"

export const BrandBetLimitCheckboxGroupTable = ({
  name = "betLimits",

  width = "100%",
  disabled = false,
  ...props
}) => {
  let { control, setValue, watch, register, getValues } = useFormContext()
  const { fields } = useFieldArray({
    control,
    name,
    keyName: "_id"
  })

  let selection = watch(name)
  let allChecked = isAllPropChecked(selection)

  return useMemo(() => {
    let fieldIndexPairs = fields.map((field, index) => [field, index])
    return (
      <Set orientation="vertical" width={width}>
        <Table width={width}>
          <Table.Head>
            <BetLimitCheckboxGroupTableHeadRow
              name={name}
              fields={fields}
              checkedFieldName={checkedFieldName}
              allChecked={allChecked}
              onCheckedChange={(checked) => {
                if (checked) {
                  //auto set first currency default
                  applyFirstCurrencyPairs((field, index) => {
                    let fieldName = defaultFieldName(name, field, index)
                    setValue(fieldName, true)
                  }, fieldIndexPairs)
                } else {
                  //remove all default
                  fieldIndexPairs.forEach(([field, index]) => {
                    let fieldName = defaultFieldName(name, field, index)
                    setValue(fieldName, false)
                  })
                }
              }}
              disabled={disabled}
              addonAfter={
                <Table.HeadCell textAlign="center">
                  <Trans zh="预设" en="Default" />
                </Table.HeadCell>
              }
            />
          </Table.Head>
          <Table.Body>
            {fields.map((field, index) => {
              let isDefaultFieldName = defaultFieldName(name, field, index)
              return (
                <BetLimitCheckboxGroupTableRow
                  key={field._id}
                  name={name}
                  disabled={disabled}
                  field={field}
                  index={index}
                  checkedFieldName={checkedFieldName}
                  onCheckedChange={(checked) => {
                    if (checked) {
                      let foundDefault = !!findDefault(true, fieldIndexPairs, {
                        excludedId: field.id
                      })
                      if (!foundDefault) {
                        setValue(isDefaultFieldName, true)
                      }
                    } else if (!checked) {
                      setValue(isDefaultFieldName, false)
                    }
                  }}
                  addonAfter={
                    <Table.Cell textAlign="center">
                      <Box>
                        <Switch
                          name={isDefaultFieldName}
                          switchRef={register()}
                          defaultChecked={field.isDefault}
                          justifyContent="center"
                          onChange={(event) => {
                            let checked = event.target.checked

                            if (checked) {
                              let sameCurrencyFieldPairs = filterPairsByCurrency(
                                field.currencyCode,
                                fieldIndexPairs,
                                {
                                  excludedId: field.id
                                }
                              )

                              sameCurrencyFieldPairs.forEach(
                                ([field, index]) => {
                                  let toggleChecked = !checked
                                  setValue(
                                    defaultFieldName(name, field, index),
                                    toggleChecked
                                  )
                                }
                              )
                            }
                          }}
                        />
                      </Box>
                    </Table.Cell>
                  }
                />
              )
            })}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

const applyFirstCurrencyPairs = (fn, pairs = []) => {
  // currency code -> true | false
  let applied = {}

  pairs.reduce((applied, [field, index]) => {
    let { currencyCode } = field
    if (!applied[currencyCode]) {
      fn(field, index)
      applied[currencyCode] = true
    }

    return applied
  }, applied)
}

const findDefault = (value = true, pairs = [], options) => {
  return pairs.find(([{ id, isDefault }]) => {
    let excludedId = options?.excludedId
    if (excludedId) {
      return excludedId != id && isDefault === value
    }

    return isDefault === value
  })
}

const filterPairsByCurrency = (code = "", pairs = [], options) => {
  return pairs.filter(([{ id, currencyCode }, index]) => {
    let excludedId = options?.excludedId
    if (excludedId && excludedId == id) {
      return false
    }

    return currencyCode === code
  })
}

const checkedFieldName = (name, field, index) => `${name}[${index}].checked`

const defaultFieldName = (name, field, index) => `${name}[${index}].isDefault`
