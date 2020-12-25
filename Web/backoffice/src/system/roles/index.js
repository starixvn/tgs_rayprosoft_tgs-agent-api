import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Formik, Field } from "formik"
import { Box, Button, InputField, Set, Icon } from "bumbag"
import { faCheck } from "@fortawesome/free-solid-svg-icons"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { CopyButton } from "./copy"
import { DeleteButton } from "./delete"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute,
  useUserRoleType,
  useUserRoleId
} from "../../user/context"
import { Code } from "./menu"
import { useRoleList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"

export { Code }

export const Content = ({}) => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

const AsyncContent = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    name: ""
  })
  let { pageIndex, pageSize, name } = state
  let { data, isLoading, mutate } = useRoleList(toApiParams(state))
  let userRoleId = useUserRoleId()
  let items = data?.items.filter(({ id }) => id != userRoleId)
  const fetchTable = mutate
  let columns = useTableColumns({ fetchTable })
  return (
    <PageContent
      title={<Trans zh="权限组管理" en="Roles" />}
      actionBar={
        <>
          <SearchForm
            search={(data) => {
              let newState = {
                pageIndex: 0,
                name: data?.name
              }
              if (equals(newState, { pageIndex, name })) {
                mutate()
              }
              setState(newState)
            }}
            isPending={isLoading}
          />
          <AddButton onSubmitted={fetchTable} />
        </>
      }
    >
      <Table
        controlledState={state}
        columns={columns}
        data={items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
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
            <SearchButton isLoading={isPending} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const searchInputPlaceHolder = () =>
  useTrans({
    zh: "权限组名称输入框",
    en: "Input role name"
  })

const useTableColumns = ({ fetchTable }) => {
  let columns = [
    sequenceColumn(),
    {
      id: "name",
      accessor: "name",
      Header: "权限组名称",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      width: 400
    },
    {
      id: "isRoot",
      accessor: "isRoot",
      Header: "根管理权限",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value: isRoot }) => {
        return (
          <Box alignX="center">
            {isRoot ? <Icon icon={faCheck} type="font-awesome" /> : ""}
          </Box>
        )
      }
    }
  ]

  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      disableSortBy: true,
      accessor: (row) => row,
      Header: "操作",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, name, isRoot } = value
        return (
          <Box alignX="center">
            <Set>
              <EditButton id={id} onSubmitted={fetchTable} />
              <CopyButton
                id={id}
                name={name}
                onSubmitted={fetchTable}
                disabled={isRoot}
              />
              <DeleteButton id={id} name={name} onSubmitted={fetchTable} />
            </Set>
          </Box>
        )
      }
    })
  }

  return useMemo(() => columns)
}
