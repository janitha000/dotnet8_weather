import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { ApiError, type CreateCityDto } from "../types/api";
import { cityKeys } from "../features/cities/cityKeys";
import { createCity } from "../api/citiesApi";

export function AddCityPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const numberPattern = /^-?\d+(\.\d+)?$/;

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
      // Seed cache so CityPage can hit immediately
      queryClient.setQueryData(cityKeys.byName(city.name), city);
      // Optional: mark any city lists stale later
      void queryClient.invalidateQueries({ queryKey: cityKeys.all });
      navigate(`/cities/${encodeURIComponent(city.name)}`);
    },
  });

  const serverError =
    mutation.error instanceof ApiError
      ? mutation.error.status === 409
        ? (mutation.error.problem?.detail ??
          mutation.error.message ??
          "City already exists")
        : mutation.error.status === 401
          ? "Unauthorized — please log in again"
          : mutation.error.message
      : mutation.error
        ? "Failed to create city"
        : null;

  return (
    <main>
      <h1>Add City</h1>
      <p>Requires admin login. Duplicates return 409.</p>
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
        {errors.name && <p role="alert">{errors.name.message}</p>}
        <label htmlFor="country">Country</label>
        <input
          id="country"
          {...register("country", {
            required: "Country is required",
            minLength: { value: 3, message: "Min 3 characters" },
            maxLength: { value: 100, message: "Max 100 characters" },
          })}
        />
        {errors.country && <p role="alert">{errors.country.message}</p>}
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
        {errors.latitude && <p role="alert">{errors.latitude.message}</p>}
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
        {errors.longitude && <p role="alert">{errors.longitude.message}</p>}
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
      {serverError && <p role="alert">{serverError}</p>}
    </main>
  );
}
