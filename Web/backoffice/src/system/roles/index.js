import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { Box, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import AsyncTable, { useTableState } from "../../table/async"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { CopyButton } from "./copy"
import { DeleteButton } from "./delete"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Code } from "./menu"

export { Code }

export const Content = ({}) => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

export const AsyncContent = () => {
  let { state, setState, headSortBy, headOrderBy } = useTableState({
    name: "",
    searchCount: 0
  })
  let { pageIndex, pageSize, name = "", searchCount } = state

  const deferFn = () =>
    client.get("/api/role", {
      searchParams: {
        page: pageIndex + 1,
        rows: pageSize,
        name,
        sortby: headSortBy(),
        orderby: headOrderBy()
      }
    })

  let async = useAsync({ deferFn })
  let { run: fetchTable, promise, isPending } = async

  let columns = useMemo(() =>
    tableColumns({
      fetchTable
    })
  )

  useEffect(() => {
    fetchTable()
  }, [pageIndex, pageSize, name, searchCount])

  return (
    <PageContent
      title={title()}
      actionBar={
        <>
          <SearchForm
            search={(data) => {
              setState({
                page: 0,
                searchCount: searchCount + 1,
                ...data
              })
            }}
            isPending={isPending}
          />
          <AddButton onSubmitted={fetchTable} />
        </>
      }
    >
      <AsyncTable
        state={state}
        onStateChange={setState}
        promise={promise}
        head
        body
        columns={columns}
      />
    </PageContent>
  )
}

const SearchForm = ({ search, isPending, ...props }) => {
  return (
    <Formik
      initialValues={{ name: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="name"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("name", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <Button type="submit" isLoading={isPending}>
              {searchButtonLabel()}
            </Button>
          </Set>
        </form>
      )}
    </Formik>
  )
}

//TODO: i18n
const title = () => "权限组管理"

//TODO: i18n
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "权限组名称输入框"
const tableColumns = ({ fetchTable }) => [
  sequenceColumn(),
  {
    Header: "权限组名称",
    id: "name",
    accessor: "name",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    width: 400
  },
  {
    disableSortBy: true,
    accessor: (row) => {
      return {
        id: row.id,
        name: row.name
      }
    },
    Header: "操作",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    Cell: ({ value }) => {
      let { id, name } = value
      return (
        <Box alignX="center">
          <Set>
            <EditButton id={id} onSubmitted={fetchTable} />
            <CopyButton id={id} name={name} onSubmitted={fetchTable} />
            <DeleteButton id={id} name={name} onSubmitted={fetchTable} />
          </Set>
        </Box>
      )
    }
  }
]
