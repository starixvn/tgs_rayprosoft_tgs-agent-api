import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
  Box,
  Set,
  Flex,
  Button,
  Text,
  Switch,
  Input,
  Stack,
  Dialog
} from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute,
  useUserBrandId,
  useUserRoleType
} from "../../user/context"
import { useChangeAnnouncementOrders, useAnnouncementList } from "./client"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { Trans } from "../../i18n"
import {
  from as statusFrom,
  Label as StatusLabel,
  InVerify,
  InVerifyName,
  Effective,
  EffectiveName,
  Expired,
  ExpiredName,
  Rejected,
  RejectedName,
  InvalidName
} from "./status"
import { AddButton, VerifyButton, EditButton, ViewButton } from "./edit"
import {
  Verify as VerifyMenu,
  View as ViewMenu,
  Update as UpdateMenu
} from "./menu"
import AlertDialog from "../../dialog/Alert"
import { ToggleButton as StatusToggleButton } from "./status"
import { useFormContext, useForm, FormProvider } from "react-hook-form"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { nextRoleType } from "../../system/roles/type"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

const Body = ({}) => {
  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])

  let userBrandId = useUserBrandId()
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    brandId: userBrandId,
    statusName: hasVerifyAuth
      ? InVerifyName
      : hasViewAuth
      ? EffectiveName
      : null
  })
  let { statusName } = state
  let isEffectiveTab = statusName === EffectiveName
  let { data = {}, isLoading, mutate, clearCache } = useAnnouncementList(
    isEffectiveTab
      ? {
          ...toApiParams(state),
          sortby: "displayOrder",
          status: [statusFrom(EffectiveName), statusFrom(InvalidName)]
        }
      : toApiParams({
          ...state,
          status: statusFrom(statusName)
        })
  )

  let tab = useTabState({
    selectedId: statusName
  })

  const activeTab = (newStatusName) => () => {
    if (newStatusName === statusName) {
      mutate()
    }

    setState({
      pageIndex: 0,
      statusName: newStatusName,
      sortBy: []
    })
  }

  return (
    <PageContent
      title={<Trans zh="公告管理" en="Announcements" />}
      actionBar={
        <ActionBar
          mutate={mutate}
          onBrandChange={(brandId) => {
            setState({
              pageIndex: 0,
              brandId
            })
          }}
        />
      }
    >
      <Tabs>
        <TabList {...tab}>
          {hasVerifyAuth && (
            <Tab
              tabId={InVerifyName}
              onClick={activeTab(InVerifyName)}
              {...tab}
            >
              <StatusLabel value={InVerify} />
            </Tab>
          )}

          <Tab
            tabId={EffectiveName}
            onClick={activeTab(EffectiveName)}
            {...tab}
          >
            <StatusLabel value={Effective} />
          </Tab>
          <Tab tabId={ExpiredName} onClick={activeTab(ExpiredName)} {...tab}>
            <StatusLabel value={Expired} />
          </Tab>
          <Tab tabId={RejectedName} onClick={activeTab(RejectedName)} {...tab}>
            <StatusLabel value={Rejected} />
          </Tab>
        </TabList>
        {hasVerifyAuth && (
          <TabPanel tabId={InVerifyName} {...tab}>
            {statusName === InVerifyName && (
              <AsyncTable
                statusName={statusName}
                data={data}
                state={state}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
                mutate={mutate}
                clearCache={clearCache}
              />
            )}
          </TabPanel>
        )}
        <TabPanel tabId={EffectiveName} {...tab}>
          {statusName === EffectiveName && (
            <EffectiveTable
              statusName={statusName}
              data={data}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              state={state}
              isLoading={isLoading}
              mutate={mutate}
              clearCache={clearCache}
              tab={tab}
            />
          )}
        </TabPanel>
        <TabPanel tabId={ExpiredName} {...tab}>
          {statusName === ExpiredName && (
            <AsyncTable
              statusName={statusName}
              data={data}
              state={state}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              isLoading={isLoading}
              mutate={mutate}
              clearCache={clearCache}
            />
          )}
        </TabPanel>
        <TabPanel tabId={RejectedName} {...tab}>
          {statusName === RejectedName && (
            <AsyncTable
              statusName={statusName}
              data={data}
              state={state}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              isLoading={isLoading}
              mutate={mutate}
              clearCache={clearCache}
            />
          )}
        </TabPanel>
      </Tabs>
    </PageContent>
  )
}

const ActionBar = ({ mutate, onBrandChange }) => {
  let roleType = useUserRoleType()
  let brandId = useUserBrandId()
  let formMethods = useForm()
  return (
    <Flex justifyContent="space-between">
      <Box>
        {!brandId && (
          <FormProvider {...formMethods}>
            <MerchantIdSelect
              start={nextRoleType(roleType)}
              onBrandChange={onBrandChange}
            />
          </FormProvider>
        )}
      </Box>
      <Box>
        <AddButton onSubmitted={mutate} />
      </Box>
    </Flex>
  )
}

const AsyncTable = ({
  statusName,
  state,
  setPageIndex,
  setSortOrder,
  data,
  isLoading,
  mutate,
  clearCache
}) => {
  let { pageSize } = state
  let columns = useColumns({
    mutate,
    clearCache,
    statusName,
    items: data?.items
  })

  return (
    <Table
      initialState={state}
      controlledState={state}
      columns={columns}
      data={data?.items}
      pageCount={pageCount(pageSize, data?.totalCount)}
      setPageIndex={setPageIndex}
      setSortOrder={setSortOrder}
      isLoading={isLoading}
    />
  )
}

const EffectiveTable = ({
  statusName,
  state,
  setPageIndex,
  setSortOrder,
  data,
  isLoading,
  mutate,
  clearCache
}) => {
  let { pageSize } = state
  let [displayOrders, setDisplayOrder] = useState({})
  let [submitData, setSubmit] = useState()

  let {
    result,
    isLoading: isChangingDisplayOrder,
    error
  } = useChangeAnnouncementOrders(submitData)

  const reset = () => {
    setDisplayOrder({})
    setSubmit(null)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      mutate()
    }
  }, [result, error])

  useEffect(() => {
    if (statusName != EffectiveName) {
      reset()
    }
  }, [statusName])

  const changeDisplayOrder = ({ id, value }) => {
    let newDisplayOrders = {
      ...displayOrders
    }
    Reflect.set(newDisplayOrders, id, value)
    setDisplayOrder(newDisplayOrders)
  }

  let columns = useColumns({
    mutate,
    clearCache,
    changeDisplayOrder,
    statusName,
    items: data?.items
  })
  return (
    <>
      {error && <AlertDialog {...error} onClickClose={() => setSubmit(null)} />}
      <Stack spacing="minor-1">
        <Table
          initialState={state}
          controlledState={state}
          columns={columns}
          data={data?.items}
          pageCount={pageCount(pageSize, data?.totalCount)}
          setPageIndex={setPageIndex}
          setSortOrder={setSortOrder}
          isLoading={isLoading}
        />
        <Dialog.Footer position="sticky" bottom="0">
          <Button
            isLoading={isChangingDisplayOrder}
            onClick={() => {
              let list = Object.entries(displayOrders).map(([key, value]) => ({
                id: key,
                displayOrder: value
              }))
              setSubmit({ orders: list })
            }}
          >
            <Trans zh="确认排序" en="Confirm Order" />
          </Button>
        </Dialog.Footer>
      </Stack>
    </>
  )
}

const useColumns = ({
  mutate,
  clearCache,
  statusName: tabStatusName,
  changeDisplayOrder,
  items = []
}) => {
  let hasUpdateAuth = useIsDisplayMenuByRoute([UpdateMenu])
  let columns = [
    tabStatusName != EffectiveName
      ? sequenceColumn()
      : {
          id: "displayOrder",
          accessor: (record) => record,
          Header: <Trans zh="序" en="Order" />,
          disableSortBy: true,
          headCell: {
            textAlign: "center"
          },
          cell: {
            textAlign: "center"
          },
          Cell: ({ value }) => {
            let { id, displayOrder = "" } = value
            return (
              <Input
                type="number"
                size="small"
                min="0"
                minLength="1"
                width="5em"
                defaultValue={displayOrder}
                onChange={(event) => {
                  let value = event.target.value
                  let number = parseInt(value)
                  if (Number.isFinite(number)) {
                    changeDisplayOrder({
                      id,
                      value: number
                    })
                  }
                }}
              />
            )
          }
        },
    {
      accessor: "subject",
      Header: <Trans zh="主题" en="Subject" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "createdByUsername",
      Header: <Trans zh="操作人" en="Operator" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      accessor: "updatedOn",
      Header: <Trans zh="最近更新时间" en="Last Update" />,
      disableSortBy: true
    }),
    dateTimeColumn({
      accessor: "from",
      Header: <Trans zh="开始时间" en="Start" />,
      disableSortBy: true
    }),
    dateTimeColumn({
      accessor: "to",
      Header: <Trans zh="结束时间" en="End" />,
      disableSortBy: true
    }),
    {
      id: "status",
      accessor: (record) => record,
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { status } = value
        if (tabStatusName === EffectiveName) {
          return (
            <StatusToggleButton
              record={value}
              onSubmitted={() => {
                //clearCache()
                mutate(undefined)
              }}
              disabled={!hasUpdateAuth}
            />
          )
        }

        return (
          <Text.Block
            css={css`
              white-space: nowrap;
            `}
          >
            <StatusLabel value={status} />
          </Text.Block>
        )
      }
    }
  ]

  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let hasAnyMenu = hasViewAuth
  switch (tabStatusName) {
    case InVerifyName:
      hasAnyMenu = hasVerifyAuth
      break
  }

  if (hasAnyMenu) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set alignX="center">
            {tabStatusName === InVerifyName && (
              <VerifyButton record={value} onSubmitted={mutate} />
            )}
            {tabStatusName === EffectiveName && (
              <EditButton record={value} onSubmitted={mutate} />
            )}
            {(tabStatusName === EffectiveName ||
              tabStatusName === ExpiredName ||
              tabStatusName === RejectedName) && <ViewButton record={value} />}
          </Set>
        )
      }
    })
  }
  return useMemo(() => {
    return columns
  }, [tabStatusName, JSON.stringify(items)])
}
