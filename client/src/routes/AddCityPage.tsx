import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { createCity } from "../api/citiesApi";
import { getErrorMessage } from "../api/errorMapping";
import { ErrorAlert } from "../components/ErrorAlert";
import { PageHeader } from "../components/PageHeader";
import { cityKeys } from "../features/cities/cityKeys";
import type { CreateCityDto } from "../types/api";

const numberPattern = /^-?\d+(\.\d+)?$/;

export function AddCityPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateCityDto>({
    defaultValues: {
      name: "",
      country: "",
      latitude: "",
      longitude: "",
      timeZone: "",
    },
  });

  const mutation = useMutation({
    mutationFn: createCity,
    onSuccess: (city) => {
      queryClient.setQueryData(cityKeys.byName(city.name), city);
      void queryClient.invalidateQueries({ queryKey: cityKeys.all });
      navigate(`/cities/${encodeURIComponent(city.name)}`);
    },
  });

  const serverError = mutation.error
    ? getErrorMessage(mutation.error, "Failed to create city")
    : null;

  return (
    <main>
      <PageHeader title="Add City">
        <p>Requires admin login. Duplicates return 409.</p>
      </PageHeader>

      <form
        onSubmit={handleSubmit((values) => {
          mutation.mutate(values);
        })}
        noValidate
      >
        <label htmlFor="name">Name</label>
        <input
          id="name"
          {...register("name", {
            required: "Name is required",
            minLength: { value: 3, message: "Min 3 characters" },
            maxLength: { value: 100, message: "Max 100 characters" },
          })}
        />
        {errors.name && <ErrorAlert message={errors.name.message ?? ""} />}

        <label htmlFor="country">Country</label>
        <input
          id="country"
          {...register("country", {
            required: "Country is required",
            minLength: { value: 3, message: "Min 3 characters" },
            maxLength: { value: 100, message: "Max 100 characters" },
          })}
        />
        {errors.country && (
          <ErrorAlert message={errors.country.message ?? ""} />
        )}

        <label htmlFor="latitude">Latitude</label>
        <input
          id="latitude"
          {...register("latitude", {
            required: "Latitude is required",
            pattern: {
              value: numberPattern,
              message: "Latitude must be a number",
            },
          })}
        />
        {errors.latitude && (
          <ErrorAlert message={errors.latitude.message ?? ""} />
        )}

        <label htmlFor="longitude">Longitude</label>
        <input
          id="longitude"
          {...register("longitude", {
            required: "Longitude is required",
            pattern: {
              value: numberPattern,
              message: "Longitude must be a number",
            },
          })}
        />
        {errors.longitude && (
          <ErrorAlert message={errors.longitude.message ?? ""} />
        )}

        <label htmlFor="timeZone">Time zone (optional)</label>
        <input
          id="timeZone"
          {...register("timeZone")}
          placeholder="Asia/Colombo"
        />

        <button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? "Saving…" : "Create city"}
        </button>
      </form>

      {serverError && <ErrorAlert message={serverError} />}
    </main>
  );
}
